using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.BasicData.Capacities;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.States;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Machines.Mongo;
using Wimi.BtlCore.Notices;
using Wimi.BtlCore.Visual.Dto;
using Abp.Auditing;
using System;
using System.IO;
using System.Text;
using DeviceMonitorFramework;
using MongoDB.Bson;
using Wimi.BtlCore.Configuration;
using System.Data.SqlClient;
using Dapper;
using NUglify.Helpers;
using Abp.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Wimi.BtlCore.Visual
{
    [AbpAllowAnonymous]
    public class VisualAppService : BtlCoreAppServiceBase, IVisualAppService
    {
        private const string Alarm = "Alarm";
        private const string AlarmNoKey = "STD::AlarmNo";
        private const string Capacity = "Capacity";
        private const string Parameter = "Parameter";
        private const string State = "State";
        private readonly ICommonLookupAppService commonLookupAppService;
        private readonly IRepository<MachineGatherParam, long> machineGatherParamRepository;
        private readonly IRepository<Notice> noticesRepository;
        private readonly IRepository<Capacity> capacityRepository;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IRepository<State, long> stateRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly DeviceGroupManager deviceGroupManager;
        private readonly IRepository<DeviceGroup> deviceGroupRepository;
        private readonly MongoMachineManager mongoMachineManager;
        private readonly ISettingManager settingManager;
        private readonly ICommonRepository commonRepository;

        public VisualAppService(
            ICommonLookupAppService commonLookupAppService,
            IRepository<Notice> noticesRepository,
            IRepository<MachineGatherParam, long> machineGatherParamRepository,
            IRepository<Capacity> capacityRepository,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            IRepository<State, long> stateRepository,
            IRepository<Machine> machineRepository,
            DeviceGroupManager deviceGroupManager,
            IRepository<DeviceGroup> deviceGroupRepository,
            MongoMachineManager mongoMachineManager,
            ISettingManager settingManager,
            ICommonRepository commonRepository)
        {
            this.commonLookupAppService = commonLookupAppService;
            this.noticesRepository = noticesRepository;
            this.machineGatherParamRepository = machineGatherParamRepository;
            this.capacityRepository = capacityRepository;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.stateRepository = stateRepository;
            this.machineRepository = machineRepository;
            this.deviceGroupManager = deviceGroupManager;
            this.deviceGroupRepository = deviceGroupRepository;
            this.mongoMachineManager = mongoMachineManager;
            this.settingManager = settingManager;
            this.commonRepository = commonRepository;
        }

        /// <summary>
        ///     添加或者更新公告信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.Pages_VisualNotice_Manage)]
        public async Task AddOrUpdateNoticsAsync(GetNoticeInputDto input)
        {
            var entity = ObjectMapper.Map<Notice>(input);
            var existWorkShopCode =
                await
                this.noticesRepository.CountAsync(
                    n =>
                    n.Content.ToLower().Trim() == input.Content.ToLower().Trim() && n.RootDeviceGroupCode == input.RootDeviceGroupCode)
                > 0;
            if (!input.Id.HasValue && existWorkShopCode)
            {
                throw new UserFriendlyException(this.L("ThisEntryAlreadyExists"));
            }

            if (input.Id.HasValue)
            {
                var notices = await this.noticesRepository.GetAsync(input.Id.Value);
                notices.Content = input.Content;
                notices.IsActive = input.IsActive;
                notices.RootDeviceGroupCode = input.RootDeviceGroupCode;
            }
            else
            {
                await this.noticesRepository.InsertAsync(entity);
            }

            await this.AdjustNoticeActiveAsync(entity, false);
        }

        /// <summary>
        ///     删除公告信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.Pages_VisualNotice_Manage)]
        public async Task DeleteNotices(EntityDto input)
        {
            if (input.Id != 0)
            {
                await this.noticesRepository.DeleteAsync(input.Id);
            }
        }

        [HttpPost]
        public async Task<GetNoticeOutputDto> GetNoticesForVisual(GetNoticeInputDto input)
        {
            var query = (await this.noticesRepository.GetAllListAsync()).Where(
                      n => n.IsActive && n.RootDeviceGroupCode == input.RootDeviceGroupCode)
                      .OrderByDescending(n => n.CreationTime)
                      .FirstOrDefault();
            if (query == null)
            {
                return new GetNoticeOutputDto();
            }

            return ObjectMapper.Map<GetNoticeOutputDto>(query);
        }

        /// <summary>
        ///     查询已发布的公告信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.Pages_VisualNotice)]
        [HttpPost]
        public async Task<PagedResultDto<GetNoticeOutputDto>> GetNoticesList(GetNoticeInputDto input)
        {
            var query = (from n in this.noticesRepository.GetAll()
                         join w in this.deviceGroupRepository.GetAll() on n.RootDeviceGroupCode equals w.Code
                         join u in this.UserManager.Users on n.CreatorUserId equals u.Id
                         select
                             new GetNoticeOutputDto
                             {
                                 Content = n.Content,
                                 WorkShopCode = n.RootDeviceGroupCode,
                                 WorkShopName = w.DisplayName,
                                 IsActive = n.IsActive,
                                 Id = n.Id,
                                 CreatorUserId = n.CreatorUserId,
                                 CreatorUserName = u.Name,
                                 CreationTime = n.CreationTime
                             }).WhereIf(
                                     !string.IsNullOrWhiteSpace(input.Search.Value),
                                     s => s.Content.Contains(input.Search.Value));

            var resutl = await query.OrderBy(input.Sorting).AsNoTracking().PageBy(input).ToListAsync();

            var noticesList = new List<GetNoticeOutputDto>();
            ObjectMapper.Map(resutl, noticesList);
            var noticesCount = await query.CountAsync();

            return new DatatablesPagedResultOutput<GetNoticeOutputDto>(
                noticesCount,
                noticesList,
                noticesCount)
            {
                Draw = input.Draw
            };
        }

        /// <summary>
        ///     获取实时设备采集数据 从Mongo读取
        /// </summary>
        /// <param name="input">设备编号</param>
        /// <returns></returns>
        [DisableAuditing]
        [HttpPost]
        public IEnumerable<GetRealtimeMachineInfoOutputDto> GetRealtimeMachineInfoFromMongo(
            GetRealtimeMachineInfoInputDto input)
        {
            var mongodata = mongoMachineManager.ListOriginalMongoMachinesBsonDocument(input.MacNoList);

            if (!mongodata.Any())
            {
                return new List<GetRealtimeMachineInfoOutputDto>();
            }

            try
            {
                var result = mongodata.Select(
                    m => new GetRealtimeMachineInfoOutputDto
                    {
                        Code = m["MachineCode"].AsString,
                        Name = m["Name"].AsString,
                        DataItemInfo = this.SetDataItemInfos(m)
                    }).ToList();
                return result;
            }
            catch (Exception e)
            {
                this.Logger.Fatal("获取实时设备采集数据", e);
            }

            return new List<GetRealtimeMachineInfoOutputDto>();
        }

        [DisableAuditing]
        [HttpPost]
        public async Task<GetShiftEffDto> GetShiftEffDto(EntityDto<string> input)
        {
            try
            {
                return new GetShiftEffDto
                {
                    ShiftEffSummarys = await commonRepository.GetShiftEffSummyList(input.Id),
                    UtilizationRate = await commonRepository.GetShiftUtilizationRateList(input.Id)
                };
            }
            catch (Exception e)
            {
                this.Logger.Fatal("visual-设备效率", e);
            }

            return new GetShiftEffDto();
        }

        [DisableAuditing]
        public async Task<IEnumerable<YieldsPerHourDto>> ListYieldsPerHour(EntityDto<string> input)
        {
            var returnValue = new List<YieldsPerHourDto>();
            try
            {
                var query = await (from c in this.capacityRepository.GetAll()
                                   join mdg in this.machineDeviceGroupRepository.GetAll() on c.MachineId equals mdg.MachineId
                                   where c.StartTime >= DateTime.Today && c.StartTime <= DateTime.Now && mdg.DeviceGroupCode == input.Id
                                   select new { c.StartTime.Value.Hour, c.Yield }).GroupBy(
                                q => q.Hour,
                                (key, g) => new { Hour = key + 1, Yields = g.ToList() }).ToListAsync();

                for (var i = 1; i <= 24; i++)
                {
                    var hourDto = query.FirstOrDefault(n => n.Hour == i);
                    returnValue.Add(
                        new YieldsPerHourDto() { Hour = i, Yield = hourDto?.Yields.Sum(n => n.Yield) ?? 0 });
                }
            }
            catch (Exception e)
            {
                this.Logger.Fatal("visual - 每小时产量", e);
            }

            return returnValue;
        }

        [DisableAuditing]
        public async Task<IEnumerable<StateRatioDto>> ListStateRatio(EntityDto<string> input)
        {
            try
            {
                var query = await (from s in this.stateRepository.GetAll()
                                   join m in this.machineRepository.GetAll() on s.MachineId equals m.Id
                                   join mdg in this.machineDeviceGroupRepository.GetAll() on s.MachineId equals mdg.MachineId
                                   where s.StartTime >= DateTime.Today && s.StartTime <= DateTime.Now && mdg.DeviceGroupCode == input.Id
                                   select new StateRatioDto()
                                   {
                                       MachineName = m.Name,
                                       Duration = s.Duration,
                                       Code = s.Code
                                   }).GroupBy(
                                q => q.MachineName,
                                (key, g) => new { MachineName = key, States = g.ToList() }).ToListAsync();

                return query.Select(
                    q => new StateRatioDto()
                    {
                        MachineName = q.MachineName,
                        FreeDuration = this.SetStateRatioItem(q.States, EnumMachineState.Free),
                        Offlinetion =
                                     this.SetStateRatioItem(q.States, EnumMachineState.Offline),
                        RunDuration = this.SetStateRatioItem(q.States, EnumMachineState.Run),
                        StopDuration = this.SetStateRatioItem(q.States, EnumMachineState.Stop)
                    });
            }
            catch (Exception e)
            {
                this.Logger.Fatal("看板-用时比率", e);
            }

            return new List<StateRatioDto>();
        }

        [DisableAuditing]
        [HttpPost]
        public async Task<List<GetWorkShopsDto>> GetWorkShops()
        {
            // 根级组作为车间组，在看板上分类显示
            var rootDeviceGroups = (await deviceGroupManager.FindChildrenAsync(null, false)).OrderBy(s => s.Seq);
            List<GetWorkShopsDto> workshopDtos = new List<GetWorkShopsDto>();
            foreach (var rootDeviceGroup in rootDeviceGroups)
            {
                workshopDtos.Add(new GetWorkShopsDto
                {
                    Id = rootDeviceGroup.Id,
                    Code = rootDeviceGroup.Code,
                    Name = rootDeviceGroup.DisplayName
                });
            }

            return workshopDtos;
        }

        [DisableAuditing]
        public string ReadFile(ReadFileDto input)
        {
            var sourcestring = string.Empty;
            var filename = $"{input.FilePath}/{input.FileName}";
            if (File.Exists(filename))
            {
                sourcestring = File.ReadAllText(filename, Encoding.UTF8);
            }

            return sourcestring;
        }

        /// <summary>
        ///     启用 or 不启用编辑
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateNoticsActive(EntityDto input)
        {
            var notices = await this.noticesRepository.GetAsync(input.Id);
            if (notices != null)
            {
                await this.AdjustNoticeActiveAsync(notices);
                notices.IsActive = !notices.IsActive;
            }
        }

        public bool WriteConfigFile(WriteFileDto input)
        {
            try
            {
                var filename = $"{input.FilePath}/{input.FileName}";
                File.WriteAllText(filename, input.Data, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(this.L("WriteDataFailed{0}", ex.Message));
            }

            return true;
        }

        private async Task AdjustNoticeActiveAsync(Notice notices, bool isSwitch = true)
        {
            var target = isSwitch ? !notices.IsActive : notices.IsActive;
            if (target)
            {
                var noticesList =
                    await
                    this.noticesRepository.GetAll()
                        .Where(n => n.RootDeviceGroupCode == notices.RootDeviceGroupCode && n.IsActive)
                        .WhereIf(notices.Id > 0, n => n.Id != notices.Id)
                        .ToListAsync();
                if (noticesList.Count > 0)
                {
                    noticesList.ForEach(n => n.IsActive = false);
                }
            }
        }

        private string GetParamNameByCode(List<MachineGatherParam> list, string code)
        {
            var param = list.FirstOrDefault(p => p.Code.ToLower().Equals(code.ToLower()));
            return param != null ? param.Name : string.Empty;
        }

        // 五个固定参数从Mongo的固定位置获取
        // Parameter BsonArray中参数当遇到五个固定参数需要过滤，剩余数据才是真正的参数
        private List<DataItemInfo> SetDataItemInfos(BsonDocument document)
        {
            var result = new List<DataItemInfo>();
            if (document.IsBsonNull || !document.Contains("MachineId"))
            {
                return result;
            }

            // 配置参数
            var machineId = document["MachineId"].AsInt32;
            var paramList = this.machineGatherParamRepository.GetAll().Where(m => m.MachineId == machineId).ToList();

            // 状态
            if (document.Contains(State) && document[State].AsBsonDocument.Any())
            {
                var item =
                    document[State].AsBsonDocument.Elements.Where(s => s.Name.Equals("Code"))
                        .Select(
                            d => new DataItemInfo { Hot = true, Value = d.Value, Name = StateKey, Description = "状态" })
                        .FirstOrDefault();

                result.Add(item);
            }

            // 程序
            result.Add(
                new DataItemInfo
                {
                    Hot = true,
                    Value = document["ProgramName"].AsString,
                    Description = this.GetParamNameByCode(paramList, ProgramKey),
                    Name = ProgramKey
                });

            // 产量
            if (document.Contains(Capacity) && document[Capacity].AsBsonDocument.Any())
            {
                var item =
                    document[Capacity].AsBsonDocument.Elements.Where(c => c.Name.Equals("AccumulateCount"))
                        .Select(
                            d =>
                            new DataItemInfo
                            {
                                Hot = true,
                                Value = d.Value,
                                Description = this.GetParamNameByCode(paramList, CapacityKey),
                                Name = CapacityKey
                            })
                        .FirstOrDefault();

                result.Add(item);
            }

            // 报警
            if (document.Contains(Alarm) && document[Alarm].AsBsonArray.Any())
            {
                var elements = document[Alarm].AsBsonArray.FirstOrDefault();
                if (elements != null)
                {
                    result.Add(
                        new DataItemInfo
                        {
                            Hot = true,
                            Value = elements["Code"].AsString,
                            Description = this.GetParamNameByCode(paramList, AlarmNoKey),
                            Name = AlarmNoKey
                        });

                    result.Add(
                        new DataItemInfo
                        {
                            Hot = true,
                            Value = elements["Message"].AsString,
                            Description = this.GetParamNameByCode(paramList, AlarmTextKey),
                            Name = AlarmTextKey
                        });
                }
            }

            // 添加剩余参数
            result.AddRange(this.SetParamDataItemInfo(document, paramList));
            return result;
        }

        private IEnumerable<DataItemInfo> SetParamDataItemInfo(BsonDocument document, List<MachineGatherParam> list)
        {
            var paramList = new List<DataItemInfo>();
            if (document.Contains(Parameter) && document[Parameter].AsBsonDocument.Any())
            {
                var parameterArray =
                    document[Parameter].AsBsonDocument.Select(
                        a => new Tuple<string, string>(a.Name.Trim(), a.Value.ToString().Trim())).ToList();

                var fixDataItems = SettingManager.GetSettingValue(AppSettings.MachineParameter.FixedDataItems).Split(',');

                paramList.AddRange(
                    from item in parameterArray
                    where !fixDataItems.Contains(item.Item1)
                    let machineParam =
                    list.FirstOrDefault(p => p.Code.ToLower().Equals(item.Item1.ToLower()) && p.IsShowForVisual)
                    where machineParam != null
                    select
                    new DataItemInfo
                    {
                        Hot = true,
                        Value = item.Item2,
                        Description = machineParam.Name,
                        Name = item.Item1
                    });

                return paramList;
            }

            return paramList;
        }

        private decimal SetStateRatioItem(IEnumerable<StateRatioDto> states, EnumMachineState type)
        {
            var duration = states.Where(s => s.Code == type.ToString()).Sum(m => m.Duration);
            return Math.Round(duration / (60 * 60), 2);
        }
    }
}