namespace Wimi.BtlCore.RealtimeIndicators.Parameters
{
    using Abp.Application.Services.Dto;
    using Abp.Auditing;
    using Abp.Authorization;
    using Abp.AutoMapper;
    using Abp.Domain.Repositories;
    using Abp.Domain.Uow;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Wimi.BtlCore.Authorization;
    using Wimi.BtlCore.BasicData.Alarms.Manager;
    using Wimi.BtlCore.BasicData.Alarms.Mongo;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.BasicData.Dto;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.BasicData.Machines.Manager;
    using Wimi.BtlCore.BasicData.StateInfos;
    using Wimi.BtlCore.BTLMongoDB.Repositories;
    using Wimi.BtlCore.Common;
    using Wimi.BtlCore.Common.Dto;
    using Wimi.BtlCore.CommonEnums;
    using Wimi.BtlCore.Dto;
    using Wimi.BtlCore.Extensions;
    using Wimi.BtlCore.Machines.Mongo;
    using Wimi.BtlCore.Parameters.Dto;
    using Wimi.BtlCore.Parameters.Mongo;
    using Wimi.BtlCore.RealtimeIndicators.Parameters.Dto;
    using Wimi.BtlCore.RealtimeIndicators.Parameters.Export;
    using Wimi.BtlCore.States.Mongo;


    /// <summary>
    ///     The paramters app service.
    /// </summary>
    [AbpAuthorize(PermissionNames.Pages_HistoryParameters)]
    public class ParamtersAppService : BtlCoreAppServiceBase, IParamtersAppService
    {
        /// <summary>
        ///     The defult color.
        /// </summary>
        private const string DefultColor = "#204D74";

        private readonly IParamtersExporter exporter;
        private readonly ICommonLookupAppService commonLookupAppService;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IRepository<MachineGatherParam, long> machineGatherParamRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<StateInfo> statusInfoRepository;
        private readonly IMachineManager machineManager;
        private readonly MongoAlarmManager mongoAlarmManager;
        private readonly MongoParameterManager mongoParameterManager;
        private readonly MongoMachineManager mongoMachineManager;
        private readonly MongoDbRepositoryBase<MongoParameter> mongoParameterRepository;
        private readonly AlarmManager alarmManager;

        public ParamtersAppService(
            IParamtersExporter exporter,
            IRepository<MachineGatherParam, long> machineGatherParamRepository,
            IRepository<Machine> machineRepository,
            ICommonLookupAppService commonLookupAppService,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            IRepository<StateInfo> statusInfoRepository,
            IMachineManager machineManager,
            MongoAlarmManager mongoAlarmManager,
            MongoParameterManager mongoParameterManager,
            MongoMachineManager mongoMachineManager,
            MongoDbRepositoryBase<MongoParameter> mongoParameterRepository,
            AlarmManager alarmManager)
        {
            this.exporter = exporter;
            this.machineGatherParamRepository = machineGatherParamRepository;
            this.machineRepository = machineRepository;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.commonLookupAppService = commonLookupAppService;
            this.statusInfoRepository = statusInfoRepository;
            this.machineManager = machineManager;
            this.mongoAlarmManager = mongoAlarmManager;
            this.mongoParameterManager = mongoParameterManager;
            this.mongoMachineManager = mongoMachineManager;
            this.mongoParameterRepository = mongoParameterRepository;
            this.alarmManager = alarmManager;
        }

        [HttpPost]
        public async Task<MachineDto> GetDefaultMachineByTenantId(NullableIdDto input)
        {
            using (this.CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var query = (await this.machineRepository.GetAllListAsync()).FirstOrDefault();
                return ObjectMapper.Map<MachineDto>(query);
            }
        }

        [HttpPost]
        public async Task<GetHistoryParamtersListDto> GetHistoryParamtersColumns(HistoryParamtersInputDto input)
        {
            var returnValue = new GetHistoryParamtersListDto();
            var paramList = await this.GetMachineGatherParam(input.MachineId);
            returnValue.ParamColumns = paramList
                .Select(p => new DataTablesColumns { Data = p.Code.ToLower(), Title = p.Name })
                .ToList();
            return returnValue;
        }

        [HttpPost]
        public async Task<PagedResultDto<GetHistoryParamtersDataTableDto>> GetHistoryParamtersList(HistoryParamtersInputDto input)
        {
            var request = new HistoryParameterListRequestDto()
            {
                MachineId = input.MachineId,
                ObjectId = input.ObjectId,
                StartTime = input.StartTime,
                EndTime = input.EndTime,
                Start = input.Start,
                Length = input.Length,
                PageDown = input.PageDown
            };
            var paramList = await this.GetMachineGatherParam(input.MachineId);

            var rowDatas = mongoParameterManager.GetHistoryParamters(request, paramList, out var totalCount);

            return new DatatablesPagedResultOutput<GetHistoryParamtersDataTableDto>(
                       totalCount,
                       rowDatas.ToList(),
                       totalCount)
            {
                Draw = input.Draw
            };
        }

        private async Task<GetHistoryParamtersListExportDto> GetHistoryParamtersForExport(HistoryParamtersInputDto input)
        {
            var result = new GetHistoryParamtersListExportDto();

            var request = new HistoryParameterListRequestDto()
            {
                MachineId = input.MachineId,
                ObjectId = input.ObjectId,
                StartTime = input.StartTime,
                EndTime = input.EndTime,
                Start = 0,
                Length = null,
                PageDown = input.PageDown
            };

            result.ParamList = await this.GetMachineGatherParam(input.MachineId); ;

            result.Parameters = mongoParameterManager.GetHistoryParamtersForExport(request, result.ParamList);

            result.MachineName = this.machineRepository.FirstOrDefault(x => x.Id == input.MachineId)?.Name ?? "";

            return result;

        }

        [HttpPost]
        public async Task<List<GetParamtersListDto>> GetLastNRecords(FindMachineInfoFromMongoInputDto input)
        {
            List<MachineGatherParam> machineParam;

            using (this.CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                machineParam = await this.machineGatherParamRepository.GetAll()
                                   .Where(c => c.MachineId == input.MachineId && c.IsShowForParam)
                                   .ToListAsync();
            }

            var result = mongoParameterManager.GetLastNRecords(input.MachineId, machineParam);

            return result.ToList();
        }

        /// <summary>
        ///     The get loding more alarm.
        /// </summary>
        /// <param name="input">
        ///     The input.
        /// </param>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        /// </returns>
        [HttpPost]
        public IEnumerable<MachineAlarmDto> GetLodingMoreAlarm(AlarmPagesInputDto input)
        {
            return this.GetMachineHistoryAlarm(input);
        }

        /// <summary>
        ///     The get machine status detail.
        /// </summary>
        /// <param name="input">
        ///     The input.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [DisableAuditing]
        [HttpPost]
        public async Task<MachineStatusListDto> GetMachineStatusDetail(FindMachineInfoFromMongoInputDto input)
        {
            var machineListDto = new MachineStatusListDto();
            var machine = await this.machineRepository.FirstOrDefaultAsync(s => s.Id == input.MachineId);
            var mongoMachineInfo = await this.GetRealTimeMachineInfoFromMongo(machine.Code);
            if (mongoMachineInfo != null) machineListDto.MongoMachineInfo = mongoMachineInfo;

            machineListDto.StatusInfo =
                await this.statusInfoRepository.FirstOrDefaultAsync(s => s.Code == mongoMachineInfo.Status);
            machineListDto.ImagePath = this.GetMachineImagePath(machine.ImageId);
            machineListDto.Machine = machine;
            return machineListDto;
        }

        [DisableAuditing]
        [HttpPost]
        public async Task<MachineStateOutputDto> GetMachineStatusDetailList(MachineStateInputDto input)
        {
            var inputMachineIds = await this.FormartMachineStateInputIds(input);
            var query = await (from m in this.machineRepository.GetAll()
                        join dg in this.machineDeviceGroupRepository.GetAll() on m.Id equals dg.MachineId
                        where inputMachineIds.Contains(dg.MachineId) && m.IsActive
                        select new { m, GroupId = dg.DeviceGroupId }).ToListAsync();

            var machineGroupList = query
                .GroupBy(
                    m => m.m.Id,
                    (key, g) => new { machineId = key, List = g });

            var targets = machineGroupList.Select(m => m.List.FirstOrDefault()).OrderBy(m => m.m.SortSeq).ThenBy(m => m.m.Code)
                .ThenBy(m => m.m.Id);

            var machineList = targets.Skip((input.PageNo - 1) * input.PageSize)
                                  .Take(input.PageSize)
                                  .ToList();

            var result = new MachineStateOutputDto() { TotalCount = targets.Count() };

            try
            {
                foreach (var key in machineList)
                {
                    var machine = key.m;
                    var machineGroupIdList = machineGroupList.First(g => g.machineId == machine.Id)
                        .List.Select(g => g.GroupId)
                        .ToList();
                    var dto = new MachineStatusListDto
                    {
                        GroupId = machineGroupIdList,
                        Name = machine.Name,
                        Code = machine.Code,
                        MachineId = machine.Id,
                        MongoMachineInfo = await this.GetRealTimeMachineInfoFromMongo(machine.Code),
                        ImagePath = this.GetMachineImagePath(machine.ImageId)
                    };

                    if (dto.MongoMachineInfo?.Status != null
                        && Enum.IsDefined(typeof(EnumMachineState), dto.MongoMachineInfo.Status))
                        dto.StatusInfo = await this.statusInfoRepository.FirstOrDefaultAsync(s => s.Code == dto.MongoMachineInfo.Status);

                    result.Items.Add(dto);
                }
            }
            catch (Exception e)
            {
                this.Logger.Fatal("设备实时状态", e);
            }

            return result;
        }

        /// <summary>
        ///     从MongoDB获取实时参数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableAuditing]
        [HttpPost]
        public GetParamtersListDto GetParamtersList(ParamtersInputDto input)
        {
            var machineEntity = this.machineRepository.FirstOrDefault(m => m.Id == input.MachineId);
            if (machineEntity == null) return new GetParamtersListDto();

            var mongoData = mongoMachineManager.GetMachineInfoFromMongo(machineEntity.Code);
            if (mongoData == null) return new GetParamtersListDto();

            var machineParam = this.machineGatherParamRepository.GetAll()
                .Where(c => c.MachineId == machineEntity.Id && c.IsShowForParam);

            var machineStates = new MongoState();
            if (!string.IsNullOrEmpty(mongoData.State.Code))
                machineStates = new MongoState()
                {
                    Code = mongoData.State.Code,
                    CreationTime = mongoData.State.CreationTime
                };

            var mongDataBson = BsonSerializer.Deserialize<BsonDocument>(JsonConvert.SerializeObject(mongoData));

            return new GetParamtersListDto
            {
                BlockChartParamtersList =
                               this.mongoParameterManager.SetBlockChartParamtersList(
                                   mongDataBson,
                                   machineParam,
                                   machineStates.Code),
                GaugeParamtersList =
                               this.mongoParameterManager.SetGaugeParamtersList(
                                   mongDataBson,
                                   machineParam,
                                   machineStates.Code),
                LineChartParamtersList =
                                this.mongoParameterManager.SetLineChartParamtersList(
                               mongDataBson,
                               machineParam,
                               machineStates.Code)
            };
        }

        /// <summary>
        ///     The get real time alarm list.
        /// </summary>
        /// <param name="input">
        ///     The input.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [DisableAuditing]
        [HttpPost]
        public async Task<MachineRealtimeAlarmOutDto> GetRealTimeAlarmList(AlarmPagesInputDto input)
        {
            var tempMachineCode = input.MachineCode;
            var queryMachinesExpression
                = from m in this.machineRepository.GetAll()
                  join dg in this.machineDeviceGroupRepository.GetAll() on m.Id equals dg.MachineId
                  where input.GroupIdList.Contains(dg.DeviceGroupId) && m.IsActive
                  orderby m.SortSeq, m.Code
                  select new { MachineDto = m, GroupId = dg.DeviceGroupId };

            var machineList = await queryMachinesExpression.ToListAsync();

            var groupList = machineList.GroupBy(
                m => m.MachineDto.Id,
                (key, g) => new { machineId = key, List = g.ToList() }).ToList();

            var list = machineList.Select(m => m.MachineDto).Distinct().OrderBy(m => m.SortSeq).ToList();

            var target = list.Skip((input.CurrentPageNo - 1) * input.CurrentPageSize).Take(input.CurrentPageSize);
            var returnValue = new MachineRealtimeAlarmOutDto() { TotalCount = list.Count };

            foreach (var key in target)
            {
                // 当input 输入MachineCode不为空时,只返回该MachineCode的数据,如果没有则为全部数据
                if (!string.IsNullOrEmpty(tempMachineCode)
                    && !tempMachineCode.ToLower().Equals(key.Code.ToLower())) continue;

                input.MachineCode = key.Code;
                var targetMachine = groupList.FirstOrDefault(g => g.machineId == key.Id);
                if (targetMachine != null)
                {
                    var machineGroupIdList = targetMachine
                        .List.Select(g => g.GroupId)
                        .ToList();
                    var alarm = new GetMachineAlarmOutputDto
                    {
                        MachineId = key.Id,
                        Code = key.Code,
                        Name = key.Name,
                        GroupId = machineGroupIdList,
                        ImagePath = this.GetMachineImagePath(key.ImageId),
                        MongoAlarmInfo = this.GetMongoAlarmsInfo(input)
                    };

                    // 添加状态信息
                    if (alarm.MongoAlarmInfo != null)
                        alarm.StatusInfo =
                            await this.statusInfoRepository.FirstOrDefaultAsync(
                                s => s.Code == alarm.MongoAlarmInfo.StateCode);

                    returnValue.Items.Add(alarm);
                }
            }

            return returnValue;
        }

        public async Task<IEnumerable<NameValueDto>> ListStates()
        {
            return await this.statusInfoRepository.GetAll()
                       .Where(s => s.IsStatic && s.Type == EnumMachineStateType.State)
                       .OrderBy(s => s.Id)
                       .Select(s => new NameValueDto() { Name = s.DisplayName, Value = s.Code }).ToListAsync();
        }

        public async Task<IEnumerable<ListMachineStatesDto>> ListMachineStates()
        {
            return await this.statusInfoRepository.GetAll()
                       .Where(s => s.IsStatic && s.Type == EnumMachineStateType.State)
                       .OrderBy(s => s.Id)
                       .Select(s => new ListMachineStatesDto() { DisplayName = s.DisplayName, Code = s.Code, Hexcode = s.Hexcode }).ToListAsync();
        }

        private static MongoState GetStatus(MongoMachine mongoData)
        {
            var machineStates = new MongoState();
            if (mongoData.State != null)
            {
                machineStates = new MongoState()
                {
                    Code = mongoData.State.Code,
                    CreationTime = mongoData.State.CreationTime
                };
            }

            return machineStates;
        }

        private void AdjustParamItemsData(MongoMachineInfoDto result, MongoMachine mongoData)
        {
            // 替换WorkCount和ProgramName值
            var capacityDocument = mongoData.Capacity ?? null;
            var workcount = result.ParamsItemList.FirstOrDefault(c => c.Code.ToLower().Equals(CapacityKey.ToLower()));

            if (workcount != null)
            {
                //result.ParamsItemList.Remove(workcount);
                var index = result.ParamsItemList.FindIndex(p => p.Code == workcount.Code);

                if (capacityDocument != null)
                {
                    result.ParamsItemList[index].Value = capacityDocument.OriginalCount.ToString();
                    //workcount.Value = capacityDocument.OriginalCount.ToString();
                    //result.ParamsItemList.Add(workcount);
                }
            }

            var program = result.ParamsItemList.FirstOrDefault(c => c.Code.ToLower().Equals(ProgramKey));
            if (program == null) return;
            result.IsHadProgram = true;
            result.ParamsItemList.Remove(program);

            if (mongoData.ProgramName != null)
            {
                program.Value = mongoData.ProgramName;
                result.ProgramName = program.Value;
            }

            // 从Mongo中获取程序产量
            if (capacityDocument != null)
            {
                result.ProgramCount = capacityDocument.CurrentProgramCount.ToString();
            }

        }

        private async Task<IEnumerable<MachineGatherParam>> GetMachineGatherParam(int? machineId)
        {
            var machineParam = new List<MachineGatherParam>();

            if (!machineId.HasValue) return machineParam;

            var machine = await this.machineRepository.FirstOrDefaultAsync(m => m.Id == machineId.Value);
            if (machine == null) return machineParam;

            machineParam = await this.machineGatherParamRepository.GetAll()
                .Where(c => c.MachineId == machine.Id && c.IsShowForStatus)
                .OrderBy(m => m.SortSeq)
                .ToListAsync();

            if (machineParam.Count > 0)
            {
                machineParam.Insert(0, new MachineGatherParam { Code = "creationTime", Name = "采集时间" });
            }

            return machineParam;
        }

        private IEnumerable<MachineAlarmDto> GetMachineHistoryAlarm(AlarmPagesInputDto input)
        {
            var alarmList = new List<MachineAlarmDto>();

            var docList = this.alarmManager.GetMachineHistoryAlarm(input.MachineCode, input.PageNo, input.PageSize);

            alarmList.AddRange(
                docList.Select(
                    d =>
                    {
                        var creationTime = Convert.ToDateTime(d.StartTime);
                        var code = d.Code ?? null;
                        var message = d.Message ?? null;

                        return new MachineAlarmDto
                        {
                            IsAlarming = this.IsAlarming(input.MachineCode, creationTime,code),
                            Code = code ?? string.Empty,
                            Message = message ?? string.Empty,
                            CreationTime = creationTime.ToString("yy-MM-dd HH:mm")
                        };
                    }));
            return alarmList;
        }

        private string GetMachineImagePath(Guid? imageId)
        {
            return this.machineManager.GetMachineImagePath(new EntityDto<Guid?> { Id = imageId });
        }

        private MongoAlarmInfo GetMongoAlarmsInfo(AlarmPagesInputDto input)
        {
            var mongoAlarmInfo = new MongoAlarmInfo();

            // step0: 获取当前设备信息
            var momgoData = mongoMachineManager.GetMachineInfoFromMongo(input.MachineCode);

            if (momgoData == null) return null;

            // setp1:获取当前状态
            var stateCode = "-1";
            var stateDocument = momgoData.State ?? null;
            if (stateDocument != null) stateCode = stateDocument.Code;

            mongoAlarmInfo.StateCode = stateCode;

            // setp2:获取报警信息
            if (mongoAlarmInfo.AlarmItems.Count < input.PageSize)
                mongoAlarmInfo.AlarmItems.AddRange(this.GetMachineHistoryAlarm(input));

            return mongoAlarmInfo;
        }

        private MongoMachineInfoDto GetParamsDataItems(
            MongoMachine mongoData,
            IEnumerable<MachineGatherParam> machineGatherParam,
            string stateCode)
        {
            var result = new MongoMachineInfoDto();
            var needIgnoredItems = new[] { AlarmNokey.ToLower(), AlarmTextKey.ToLower() };

            var paramDocument = mongoData.Parameter ?? null;
            if (paramDocument == null) return result;

            var jsonstr = JsonConvert.SerializeObject(paramDocument);
            var parameterbsonDocument = BsonSerializer.Deserialize<BsonValue>(jsonstr).AsBsonDocument;

            var parameterArray = parameterbsonDocument
                .Select(a => new Tuple<string, string>(a.Name.Trim(), a.Value.ToString().Trim()))
                .ToList();

            foreach (var item in machineGatherParam)
            {
                if (needIgnoredItems.Contains(item.Code.ToLower())) continue;
                var targetParam = parameterArray.FirstOrDefault(p => p.Item1 == item.Code);

                var paramItem = new MachineDetailItem()
                {
                    MachineId = item.MachineId,
                    MachineCode = item.MachineCode,
                    Name = item.Name,
                    Code = item.Code,
                    Value =
                        targetParam != null
                        && stateCode != EnumMachineState.Offline.ToString()
                            ? targetParam.Item2
                            : item.DisplayStyle == EnumParamsDisplayStyle.BlockChart
                                ? "--"
                                : "0",
                    Unit = item.Unit
                };

                if (stateCode != null && stateCode.ToUpper().Equals(EnumMachineState.Offline.ToString().ToUpper()) &&
                    paramItem.Code.ToUpper().Equals("STD::STATUS"))
                {
                    paramItem.Value = "Offline";
                }

                result.ParamsItemList.Add(paramItem);
            }

            this.AdjustParamItemsData(result, mongoData);
            return result;
        }

        /// <summary>
        ///     从Mongo中获取设备数据
        /// </summary>
        /// <param name="machineCode"></param>
        /// <returns></returns>
        private async Task<MongoMachineInfoDto> GetRealTimeMachineInfoFromMongo(string machineCode)
        {
            var mongoData = mongoMachineManager.GetMachineInfoFromMongo(machineCode);

            if (mongoData == null) return new MongoMachineInfoDto();

            // 获取配置项
            var machineGatherParam = await this.machineGatherParamRepository.GetAll()
                                         .Where(mp => mp.MachineCode == machineCode && mp.IsShowForStatus).OrderBy(mp => mp.SortSeq)
                                         .ToListAsync();

            var machineStates = GetStatus(mongoData);
            var param = this.GetParamsDataItems(mongoData, machineGatherParam, machineStates.Code);

            // 整合数据
            return new MongoMachineInfoDto
            {
                Status = machineStates.Code,
                StatusDuration = this.SetStatusDuration(machineStates),
                AlarmItems = this.SetAlarmItems(mongoData, machineGatherParam),
                ParamsItemList = param.ParamsItemList,
                IsHadProgram = param.IsHadProgram,
                ProgramName = param.ProgramName
            };
        }

        private bool IsAlarming(string machineCode, DateTime creationTime,string code)
        {
            return mongoAlarmManager.IsAlarming(machineCode, creationTime,code);
        }

        /// <summary>
        ///     整理报警信息
        /// </summary>
        /// <param name="mongoData"></param>
        /// <param name="machineGatherParam"></param>
        /// <returns></returns>
        private IEnumerable<AlarmItem> SetAlarmItems(MongoMachine mongoData, IEnumerable<MachineGatherParam> machineGatherParam)
        {
            var alarmItemList = new List<AlarmItem>();

            var alarmDocument = mongoData.Alarm ?? null;
            var showAlarm = machineGatherParam.FirstOrDefault(m =>
                m.Code.ToLower().Contains(AlarmNokey.ToLower()) || m.Code.ToLower().Contains(PlcAlarmNokey.ToLower()));

            if (alarmDocument == null || showAlarm == null) return alarmItemList;

            foreach (var alarm in alarmDocument)
            {
                var item = new AlarmItem()
                {
                    No = alarm.Code ?? null,
                    Message = alarm.Message ?? null
                };
                alarmItemList.Add(item);
            }

            return alarmItemList;
        }

        /// <summary>
        ///     设置状态持续时间
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string SetStatusDuration(MongoState input)
        {
            var stateCreationTime = input.CreationTime?.MongoDateTimeParseExact() ?? DateTime.Now;

            var timeSpan = DateTime.Now - stateCreationTime;
            return Math.Round(timeSpan.TotalMinutes, 2).ToString(CultureInfo.InvariantCulture);
        }

        private async Task<IEnumerable<int>> FormartMachineStateInputIds(MachineStateInputDto input)
        {
            IEnumerable<int> inputMachineIds;
            switch (input.Type)
            {
                case EnumQueryMethod.ByMachine:
                    inputMachineIds = input.MachineIds;
                    break;
                case EnumQueryMethod.ByGroup:
                    inputMachineIds = await this.machineDeviceGroupRepository.GetAll()
                                          .Where(d => input.DeviceIdGroupIds.Contains(d.DeviceGroupId))
                                          .Select(d => d.MachineId).ToListAsync();
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            if (input.StateCodes == null || input.StateCodes.Length == 0) return inputMachineIds;

            var mongoMachines = mongoMachineManager.ListMongoMachine();
            var result = mongoMachines.Where(m => inputMachineIds.Contains(m.MachineId) && input.StateCodes.Contains(m.State.Code)).Select(m => m.MachineId);
            return result;
        }

        public async Task<ParamComparisonOutput> ListParamComparisonChart(ParamComparisonInputDto input)
        {
            var outPut = new ParamComparisonOutput();
            var paramerKeyList = mongoParameterManager.GetParamerNames(input.StartTime, input.EndTime);
            string creationTime = "CreationTime";

            var result = new List<ParamComparisonOutput>();

            var filter = Builders<BsonDocument>.Filter;

            var startTime = input.StartTime.PadRight(18, '0');
            var endTime = input.EndTime.PadRight(18, '0');
            var sort = Builders<BsonDocument>.Sort.Ascending("CreationTime");

            var paramNameValues = await ListNumberParamters(new EntityDto(input.MachineId));

            foreach (var paramCode in input.ParamCodes)
            {
                var documents = new List<BsonDocument>();
                var doFilter = filter.And(filter.Eq("MachineId", input.MachineId),
                    filter.Gte("CreationTime", startTime),
                    filter.Lte("CreationTime", endTime),
                    filter.Exists(paramCode)
                );

                foreach (var key in paramerKeyList)
                {
                    var collection = mongoParameterRepository.GetCollectionByName(key);
                    var document = collection.Find(doFilter)
                        .Sort(sort)
                        .ToList();
                    documents.AddRange(document);
                }

                var paramValues = documents.Select(n => n.AsBsonDocument.Elements.First(t => t.Name.Equals(paramCode)).Value);
                if (outPut.CreationTimes == null)
                {
                    outPut.CreationTimes = documents.Select(n =>
                    {
                        var createTime = n.AsBsonDocument.Elements.First(t => t.Name.Equals(creationTime)).Value.AsString;
                        var datetime = createTime.MongoDateTimeParseExact();

                        return datetime.ToString("MM-dd HH:mm:ss");
                    });
                }

                outPut.Items.Add(new ParamComparisonItem(paramNameValues.FirstOrDefault(x => x.Value.Equals(paramCode))?.Name ?? "", paramValues));
            }

            return outPut;
        }

        public async Task<IEnumerable<NameValueDto>> ListNumberParamters(EntityDto input)
        {
            var query = this.machineGatherParamRepository.GetAll()
                            .AsEnumerable()
                            .Where(m =>
                                !MachineParamKeys.Contains(m.Code) &&   //忽略固定的几个变量
                                m.DataType.Equals("number", StringComparison.OrdinalIgnoreCase) &&
                                m.IsShowForStatus &&
                                m.MachineId == input.Id)
                            .GroupBy(n => new { n.Code, n.Name })
                            
                            .Select(n => new NameValueDto() { Name = n.Key.Name, Value = n.Key.Code })
                            .ToList();
            return await Task.FromResult(query);
        }

        public async Task<FileDto> Export(HistoryParamtersInputDto input)
        {
            var st = DateTime.Now;
            Logger.Info("开始获取数据...");

            var data = await this.GetHistoryParamtersForExport(input);

            Logger.Info($"完成获取数据...{data.Parameters.Count()}...{(DateTime.Now - st).TotalSeconds}");

            st = DateTime.Now;

            var result = exporter.ExportToFile(data);

            Logger.Info($"导出获取数据...{data.Parameters.Count()}...{(DateTime.Now - st).TotalSeconds}");

            return result;
        }
    }
}