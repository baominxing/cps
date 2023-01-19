using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.ObjectMapping;
using Abp.UI;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Wimi.BtlCore.Archives;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Dto;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Machines.Manager;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Order.DefectiveParts;
using Wimi.BtlCore.Order.DefectiveReasons;
using Wimi.BtlCore.Order.PartDefects;
using Wimi.BtlCore.Order.PartDefects.Manager;
using Wimi.BtlCore.Parameters.Mongo;
using Wimi.BtlCore.Trace;
using Wimi.BtlCore.Trace.Dto;
using Wimi.BtlCore.Trace.Manager;
using Wimi.BtlCore.Trace.Repository;
using Wimi.BtlCore.Trace.Repository.Dtos;
using Wimi.BtlCore.Traceability.Dto;
using Wimi.BtlCore.Traceability.Export;

namespace Wimi.BtlCore.Traceability
{
    public class TraceAppService : BtlCoreAppServiceBase, ITraceAppService
    {
        private readonly IRepository<TraceFlowSetting> traceFlowSettingRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<MachinesShiftDetail> machineShiftDetailsRepository;
        private readonly IRepository<ShiftSolutionItem> shiftSolutionItemsRepository;
        private readonly IRepository<MachineGatherParam, long> machineGatherParamRepository;
        private readonly IRepository<TraceCatalog, long> traceCatalogRepository;
        private readonly IRepository<TraceFlowRecord, long> traceFlowRecordsRepository;
        private readonly TraceFlowSettingManager traceFlowSettingManager;
        private readonly IObjectMapper objectMapper;
        private readonly MachineManager machineManager;
        private readonly UserManager userManager;
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private readonly MongoParameterManager mongoParameterManager;
        private readonly IRepository<DeviceGroup> DeviceGroupRepository;
        private readonly IRepository<ShiftSolution> ShiftSolutionRepository;
        private readonly IRepository<DefectiveReason> DefectiveReasonRepository;
        private readonly IRepository<DefectivePart> DefectivePartRepository;
        private readonly IRepository<DefectivePartReason> DefectivePartReasonRepository;
        private readonly IRepository<PartDefect> PartDefectRepository;
        private readonly IPartDefectManager partDefectManager;
        private readonly DeviceGroupManager deviceGroupManager;
        private readonly ITraceExportManager traceExportManager;
        private readonly ITraceExporter exporter;
        private readonly IRepository<ArchiveEntry> archiveEntryRepository;
        private readonly ITraceRepository traceRepository;

        public TraceAppService(
            IRepository<TraceFlowSetting> traceFlowSettingRepository,
            IRepository<TraceCatalog, long> traceCatalogRepository,
            IRepository<TraceFlowRecord, long> traceFlowRecordsRepository,
            IRepository<Machine> machineRepository,
            IRepository<MachinesShiftDetail> machineShiftDetailsRepository,
            IRepository<ShiftSolutionItem> shiftSolutionItemsRepository,
            IRepository<MachineGatherParam, long> machineGatherParamRepository,
            TraceFlowSettingManager traceFlowSettingManager,
            IObjectMapper objectMapper,
            MachineManager machineManager,
            UserManager userManager,
            IUnitOfWorkManager unitOfWorkManager,
            MongoParameterManager mongoParameterManager,
            IRepository<DeviceGroup> DeviceGroupRepository,
            IRepository<ShiftSolution> ShiftSolutionRepository,
            IRepository<DefectiveReason> DefectiveReasonRepository,
            IRepository<DefectivePart> DefectivePartRepository,
            IRepository<PartDefect> PartDefectRepository,
            IPartDefectManager partDefectManager,
            DeviceGroupManager deviceGroupManager,
            IRepository<DefectivePartReason> DefectivePartReasonRepository,
            ITraceExportManager traceExportManager,
            ITraceExporter exporter,
            IRepository<ArchiveEntry> archiveEntryRepository,
            ITraceRepository traceRepository)
        {
            this.traceFlowSettingRepository = traceFlowSettingRepository;
            this.machineRepository = machineRepository;
            this.machineShiftDetailsRepository = machineShiftDetailsRepository;
            this.shiftSolutionItemsRepository = shiftSolutionItemsRepository;
            this.machineGatherParamRepository = machineGatherParamRepository;
            this.traceCatalogRepository = traceCatalogRepository;
            this.traceFlowRecordsRepository = traceFlowRecordsRepository;
            this.traceFlowSettingManager = traceFlowSettingManager;
            this.objectMapper = objectMapper;
            this.machineManager = machineManager;
            this.userManager = userManager;
            this.unitOfWorkManager = unitOfWorkManager;
            this.mongoParameterManager = mongoParameterManager;
            this.DeviceGroupRepository = DeviceGroupRepository;
            this.ShiftSolutionRepository = ShiftSolutionRepository;
            this.DefectiveReasonRepository = DefectiveReasonRepository;
            this.DefectivePartRepository = DefectivePartRepository;
            this.PartDefectRepository = PartDefectRepository;
            this.partDefectManager = partDefectManager;
            this.deviceGroupManager = deviceGroupManager;
            this.DefectivePartReasonRepository = DefectivePartReasonRepository;
            this.traceExportManager = traceExportManager;
            this.exporter = exporter;
            this.archiveEntryRepository = archiveEntryRepository;
            this.traceRepository = traceRepository;
        }

        /// <summary>
        /// 根据产线（根级组）查询流程列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DatatablesPagedResultOutput<TraceLineFlowSettingSummaryDto>> ListLineFlowSettings(QueryTraceLineFlowSettingDto input)
        {
            var query = traceFlowSettingRepository.GetAll().WhereIf(input.DeviceGroupId.HasValue, q => q.DeviceGroupId == input.DeviceGroupId.Value);

            var queryCount = await query.CountAsync();
            var traceCatalogList = await query.OrderBy(input.Sorting).AsNoTracking().PageBy(input).ToListAsync();
            var traceCatalogDtoList = objectMapper.Map<List<TraceLineFlowSettingSummaryDto>>(traceCatalogList);

            return new DatatablesPagedResultOutput<TraceLineFlowSettingSummaryDto>(queryCount, traceCatalogDtoList);
        }

        public async Task<List<NameValueDto>> ListLineFlowSettingsByGroupId(EntityDto deviceGroup)
        {
            var nameValueList = new List<NameValueDto>();

            var flowSettingList = await traceFlowSettingRepository.GetAll().Where(q => q.DeviceGroupId == deviceGroup.Id).OrderBy(q => q.Code).AsNoTracking().ToListAsync();

            foreach (var item in flowSettingList)
            {
                nameValueList.Add(new NameValueDto(item.DisplayName, item.Id.ToString()));
            }

            return nameValueList;
        }

        public List<NameValueDto> ListStationType()
        {
            var nameValueList = new List<NameValueDto>();

            foreach (var st in Enum.GetValues(typeof(StationType)))
            {
                nameValueList.Add(new NameValueDto { Value = ((int)st).ToString(), Name = L($"StationType-{st}") });
            }

            return nameValueList;
        }

        public List<NameValueDto> ListFlowType()
        {
            var nameValueList = new List<NameValueDto>();

            foreach (var st in Enum.GetValues(typeof(FlowType)))
            {
                nameValueList.Add(new NameValueDto { Value = ((int)st).ToString(), Name = L($"FlowType-{st}") });
            }

            return nameValueList;
        }

        [HttpPost]
        public async Task<FlowSettingsDetailDto> GetFlowSettingsDetail(EntityDto input)
        {
            var entity = await traceFlowSettingManager.GetTraceFlowSettingById(input.Id);

            var flowSettingsDetail = new FlowSettingsDetailDto
            {
                FlowSetting = objectMapper.Map<TraceFlowSettingDto>(entity),
                RelatedMachines = objectMapper.Map<List<TraceRelatedMachineDto>>(entity.RelatedMachines)
            };

            var cacheMachines = await machineManager.GetInDeviceGroupMachinesWithInactiveAsync();
            foreach (var machine in flowSettingsDetail.RelatedMachines)
            {
                machine.MachineName = cacheMachines.FirstOrDefault(q => q.Id == machine.MachineId)?.Name;
            }

            return flowSettingsDetail;
        }

        /// <summary>
        /// 保存流程，不包含关联设备
        /// </summary>
        /// <param name="traceFlowSettingDto"></param>
        /// <returns></returns>
        public async Task<FlowSettingsDetailDto> SaveTraceFlowSetting(TraceFlowSettingDto traceFlowSettingDto)
        {
            var entity = new TraceFlowSetting();
            objectMapper.Map(traceFlowSettingDto, entity);

            var newEntityId = await traceFlowSettingManager.CreateFlowSetting(entity);
            return await GetFlowSettingsDetail(new EntityDto { Id = newEntityId });
        }

        /// <summary>
        /// 更新流程设定
        /// </summary>
        /// <param name="traceFlowSettingDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task UpdateTraceFlowSetting(TraceFlowSettingDto traceFlowSettingDto)
        {
            var targetEntity = await traceFlowSettingManager.GetTraceFlowSettingById(traceFlowSettingDto.Id);

            targetEntity.DisplayName = traceFlowSettingDto.DisplayName;
            targetEntity.FlowSeq = traceFlowSettingDto.FlowSeq;
            targetEntity.FlowType = traceFlowSettingDto.FlowType;
            targetEntity.PreFlowId = traceFlowSettingDto.PreFlowId;
            targetEntity.NextFlowId = traceFlowSettingDto.NextFlowId;
            targetEntity.StationType = traceFlowSettingDto.StationType;
            targetEntity.TriggerEndFlowStyle = traceFlowSettingDto.TriggerEndFlowStyle;
            targetEntity.OfflineByQuality = traceFlowSettingDto.OfflineByQuality;
            targetEntity.QualityMakerFlowId = traceFlowSettingDto.QualityMakerFlowId;
            targetEntity.SourceOfPartNo = traceFlowSettingDto.SourceOfPartNo;
            targetEntity.NeedHandlerRelateData = traceFlowSettingDto.NeedHandlerRelateData;
            targetEntity.RelateDataSourceSettings = traceFlowSettingDto.RelateDataSourceSettings;
            targetEntity.WriteIntoPlcViaFlow = traceFlowSettingDto.WriteIntoPlcViaFlow;
            targetEntity.ContentWriteIntoPlcViaFlow = traceFlowSettingDto.ContentWriteIntoPlcViaFlow;
            targetEntity.WriteIntoPlcViaFlowData = traceFlowSettingDto.WriteIntoPlcViaFlowData;
            targetEntity.ContentWriteIntoPlcViaFlowData = traceFlowSettingDto.ContentWriteIntoPlcViaFlowData;

            await traceFlowSettingManager.UpdateFlowSetting(targetEntity);
        }

        [HttpPost]
        public async Task DeleteTraceFlowSetting(EntityDto traceFlowSetting)
        {
            await traceFlowSettingManager.DeleteFlowSettingById(traceFlowSetting.Id);
        }

        /// <summary>
        /// 设备列表
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<NameValueDto>> ListMachines(EntityDto deviceGroup)
        {
            return (await machineManager.GetInDeviceGroupMachinesWithInactiveAsync()).Where(q => q.DeviceGroupId == deviceGroup.Id).Select(q => new NameValueDto { Name = q.Name, Value = q.Id.ToString() });
        }

        /// <summary>
        /// 增加关联设备到流程中
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task AddMachineIntoTraceFlowSetting(AddMachineIntoTraceFlowSettingDto input)
        {
            var targetFlowSetting = await traceFlowSettingManager.GetTraceFlowSettingById(input.TraceFlowSettingId);

            var targetMachine = objectMapper.Map<TraceRelatedMachine>(input.RelatedMachineDto);

            var entityMachine = (await machineManager.GetInDeviceGroupMachinesWithInactiveAsync()).FirstOrDefault(q => q.Id == input.RelatedMachineDto.MachineId);
            if (entityMachine == null)
            {
                throw new UserFriendlyException(this.L("TargetDeviceDoesNotExist"));
            }

            targetMachine.MachineCode = entityMachine.Code;

            traceFlowSettingManager.AddMachineToFlowSetting(targetFlowSetting, targetMachine);
        }

        [HttpPost]
        public async Task UpdateMachineInTraceFlowSetting(AddMachineIntoTraceFlowSettingDto input)
        {
            var targetFlowSetting = await traceFlowSettingManager.GetTraceFlowSettingById(input.TraceFlowSettingId);

            var targetMachine = objectMapper.Map<TraceRelatedMachine>(input.RelatedMachineDto);

            traceFlowSettingManager.UpdateMachineInFlowSetting(targetFlowSetting, targetMachine);
        }

        /// <summary>
        /// 从流程中移除关联设备
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task RemoveMachineFromTraceFlowSetting(RemoveMachineFromTraceFlowSettingDto input)
        {
            var targetFlowSetting = await traceFlowSettingManager.GetTraceFlowSettingById(input.TraceFlowSettingId);
            var targetMachine = objectMapper.Map<TraceRelatedMachine>(input.RelatedMachineDto);

            traceFlowSettingManager.RemoveMachineFromFlowSetting(targetFlowSetting, targetMachine);
        }

        [HttpPost]
        public async Task<string> GetTraceFlowExtensionData(EntityDto flow)
        {
            return (await traceFlowRecordsRepository.GetAsync(flow.Id)).ExtensionData;
        }

        public async Task<DatatablesPagedResultOutput<TraceCatalogDto>> ListTraceCatalog(QueryTraceCatalogsDto input)
        {
            var count = 0;
            var query = new List<TraceCatalogDto>();

            var param = new QueryTraceCatalogInputDto()
            {
                MachineId = input.MachineId,
                DefectiveMachineId = input.DefectiveMachineId,
                ShiftSolutionItemId = input.ShiftSolutionItemId,
                StationCode = input.StationCode,
                Id = input.Id,
                PartNo = input.PartNo,
                DeviceGroupId = input.DeviceGroupId,
                StartFirstTime = input.StartFirstTime,
                StartLastTime = input.StartLastTime,
                EndFirstTime = input.EndFirstTime,
                EndLastTime = input.EndLastTime,
                StartDate = input.StartDate,
                EndDate = input.EndDate,
                NgPartCatlogId = input.NgPartCatlogId,
                ArchivedTable = input.ArchivedTable,
            };

            count = await traceRepository.QueryTraceCatalogCount(param);
            query = await traceRepository.QueryTraceCatalog(param, input.SkipCount, input.Length);

            return new DatatablesPagedResultOutput<TraceCatalogDto>(
                count,
                query,
                count)
            {
                Draw = input.Draw
            };
        }

        [IgnoreAntiforgeryToken]
        public async Task<TracePartDetailDto> ListTraceRecordByPartNo(QueryTraceCatalogsDto input)
        {
            var tracePartDetailDto = new TracePartDetailDto();
            if (input.PartNo.IsNullOrWhiteSpace())
            {
                return tracePartDetailDto;
            }

            var partDetail = new PartDetail();

            partDetail = await traceRepository.QueryPartDetail(input.PartNo, input.ArchivedTable);

            tracePartDetailDto.PartDetails = partDetail ?? new PartDetail();
            tracePartDetailDto.PartDetails.BuildTags();

            var traceFlowRecordsRepository = new List<TraceFlowRecord>();

            traceFlowRecordsRepository = await traceRepository.QueryTraceFlowRecord(input.PartNo, input.ArchivedTable);

            using (unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var query = (from tfr in traceFlowRecordsRepository
                             join tfs in traceFlowSettingRepository.GetAllList() on tfr.TraceFlowSettingId equals tfs.Id
                             join m in machineRepository.GetAllList() on tfr.MachineId equals m.Id
                             join u in userManager.Users on tfr.UserId equals u.Id into tfrU
                             from u in tfrU.DefaultIfEmpty()
                             where tfr.PartNo == input.PartNo
                             orderby tfr.EntryTime ascending
                             select new TraceFlowRecordDto
                             {
                                 Id = tfr.Id,
                                 PartNo = tfr.PartNo,
                                 EntryTime = tfr.EntryTime,
                                 FlowCode = tfr.FlowCode,
                                 FlowState = tfr.State,
                                 FlowTag = tfr.Tag,
                                 FlowDisplayName = tfr.FlowDisplayName,
                                 LeftTime = tfr.LeftTime,
                                 MachineCode = tfr.MachineCode,
                                 MachineId = tfr.MachineId,
                                 Station = tfr.Station,
                                 MachineName = m.Name,
                                 StationType = tfs.StationType,
                                 ContainsExtensionData = !string.IsNullOrEmpty(tfr.ExtensionData),
                                 UserName = u != null ? u.Name : ""
                             }).ToList();

                tracePartDetailDto.TraceRecords = query;
            }

            return tracePartDetailDto;
        }

        public async Task<ProcessParamterDto> ListProcessParamters(ProcessParamterRequestDto input)
        {
            var document = mongoParameterManager.GetPartsProcessParamters(input.PartNo, input.MachineId, input.OperationTimeBegin, input.OperationTimeEnd);

            var fixDataItems = SettingManager.GetSettingValue(AppSettings.MachineParameter.FixedDataItems).Split(',');

            var returnValue =
                document.Select(
                        item =>
                            item.Elements.Where(p => !fixDataItems.Contains(p.Name) && p.Value != BsonNull.Value).ToList())
                    .Cast<IEnumerable<BsonElement>>()
                    .ToList();

            var machineGatherList =
                await machineGatherParamRepository.GetAll()
                    .Where(m => m.MachineId == input.MachineId && !fixDataItems.Contains(m.Code))
                    .ToListAsync();

            return new ProcessParamterDto
            {
                ParamList = returnValue,
                MachineGatherList = ObjectMapper.Map<IEnumerable<MachineGatherParamDto>>(machineGatherList)
            };
        }

        public async Task<PagedResultDto<NgPartsResultDto>> ListNgPartsRecord(NgPartsRequestDto input)
        {
            var totalCount = await traceRepository.QueryNgPartCount(input.PartNo, input.StartTime, input.EndTime);
            var query = await traceRepository.QueryNgPart(input.PartNo, input.StartTime, input.EndTime, input.Length, input.SkipCount);

            query = query
                .WhereIf(!input.DeviceGroupId.Equals(0), q => q.DeviceGroupId.Equals(input.DeviceGroupId))
                .WhereIf(!input.PartNo.IsNullOrEmpty(), q => q.PartNo.Contains(input.PartNo))
                .WhereIf(input.MachineId != null && input.MachineId.Count() != 0, q => input.MachineId.Contains(q.MachineId))
                .WhereIf(!input.ShiftSolutionItemId.Equals(0), q => q.ShiftSolutionItemId.Equals(input.ShiftSolutionItemId))
                .WhereIf(!input.StationCode.IsNullOrWhiteSpace(), q => !string.IsNullOrEmpty(q.StationCode) && q.StationCode.Contains(input.StationCode)).ToList();

            if (string.IsNullOrEmpty(input.PartNo))
            {
                query = query.WhereIf(input.StartTime.HasValue, q => q.OnlineTime >= input.StartTime).WhereIf(input.EndTime.HasValue, q => q.OnlineTime <= input.EndTime).ToList();
            }

            var result = query.OrderBy(x => input.Sorting).ToList();
            var count = query.Count;

            var resultCount = count;
            return new DatatablesPagedResultOutput<NgPartsResultDto>(
                       totalCount,
                       result,
                       resultCount)
            {
                Draw = input.Draw
            };
        }

        public async Task<PagedResultDto<PartDefectDetailInfoDto>> ListDefectiveInfos(NgPartsRequestDto input)
        {
            var query = from pd in this.PartDefectRepository.GetAll()
                        join dpr in this.DefectivePartReasonRepository.GetAll() on new { PartId = pd.DefectivePartId, ReasonId = pd.DefectiveReasonId } equals new { dpr.PartId, dpr.ReasonId }
                        join dp in this.DefectivePartRepository.GetAll() on pd.DefectivePartId equals dp.Id
                        into pddp
                        from dp in pddp.DefaultIfEmpty()
                        join dr in this.DefectiveReasonRepository.GetAll() on pd.DefectiveReasonId equals dr.Id
                        into pddr
                        from dr in pddr.DefaultIfEmpty()
                        join u in this.UserManager.Users on pd.CreatorUserId equals u.Id
                        into pdu
                        from u in pdu.DefaultIfEmpty()
                        where pd.PartNo == input.PartNo && pd.DefectiveMachineId == input.DefectiveMachineId
                        select new PartDefectDetailInfoDto
                        {
                            DefectivePartName = dp.Name,
                            DefectiveReasonName = dr.Name,
                            CreationTime = pd.CreationTime,
                            CreatorUserName = u.Name
                        };

            var result = await query.OrderBy("DefectivePartName").PageBy(input).ToListAsync();

            var resultCount = await query.CountAsync();
            return new DatatablesPagedResultOutput<PartDefectDetailInfoDto>(
                       resultCount,
                       result,
                       resultCount)
            {
                Draw = input.Draw
            };
        }

        public async Task SaveCollectionDefects(PartDefectsCreateDto input)
        {
            foreach (var reasonId in input.DefectiveReasonsId)
            {
                var entity = ObjectMapper.Map<PartDefect>(input);
                entity.DefectiveReasonId = reasonId;
                this.partDefectManager.CheckPartDefectiveInfo(entity);
                await this.PartDefectRepository.InsertAsync(entity);
            }

        }

        public IEnumerable<NameValueDto> ListDefectiveParts()
        {
            return this.partDefectManager.ListDefectiveParts();
        }

        public IEnumerable<NameValueDto> ListDefectiveReasonsByPartId(EntityDto input)
        {
            return this.partDefectManager.ListDefectiveReasonsByPartId(input);
        }

        public async Task<IEnumerable<NameValueDto>> ListDeviceGroups()
        {
            var query = await this.deviceGroupManager.ListFirstClassDeviceGroups();
            return query.Select(x => new NameValueDto { Name = x.DisplayName, Value = x.Id.ToString() }).ToList();
        }

        public IEnumerable<NameValueDto> ListDeviceGroupMachines(EntityDto input)
        {
            return this.partDefectManager.ListMachinesByDeviceGroupId(input);
        }

        public IEnumerable<NameValueDto> ListShift()
        {
            return this.partDefectManager.ListShift();
        }

        private void CheckInputDate(QueryTraceCatalogsDto input)
        {
            if (input.EndDate == "")
            {
                input.EndDate = null;
                input.EndFirstTime = null;
                input.EndLastTime = null;
            }
            if (input.StartDate == "")
            {
                input.StartDate = null;
                input.StartFirstTime = null;
                input.StartLastTime = null;
            }
        }

        private void CheckInputDate(TraceCatalogsInputDto input)
        {
            if (input.EndDate == "")
            {
                input.EndDate = null;
                input.EndFirstTime = null;
                input.EndLastTime = null;
            }
            if (input.StartDate == "")
            {
                input.StartDate = null;
                input.StartFirstTime = null;
                input.StartLastTime = null;
            }
        }

        public async Task<FileDto> Export(QueryTraceCatalogsDto input)
        {
            var param = new QueryTraceCatalogInputDto()
            {
                MachineId = input.MachineId,
                DefectiveMachineId = input.DefectiveMachineId,
                ShiftSolutionItemId = input.ShiftSolutionItemId,
                StationCode = input.StationCode,
                Id = input.Id,
                PartNo = input.PartNo,
                DeviceGroupId = input.DeviceGroupId,
                StartFirstTime = input.StartFirstTime,
                StartLastTime = input.StartLastTime,
                EndFirstTime = input.EndFirstTime,
                EndLastTime = input.EndLastTime,
                StartDate = input.StartDate,
                EndDate = input.EndDate,
                NgPartCatlogId = input.NgPartCatlogId,
                ArchivedTable = input.ArchivedTable,
            };
            var data = await traceRepository.ListTraceExportItem(param);

            foreach (var t in data)
            {
                if (t.TraceStates == "未下线")
                {
                    t.Qualified = null;
                }
            }

            // 数据整合
            var result = data.OrderBy(s => s.EntryTime).GroupBy(p => new { p.PartNo, p.TraceStates, p.ShiftItemName, p.Qualified }).Select(t => new TraceExportItemDto
            {
                PartNo = t.Key.PartNo,
                ShiftItemName = t.Key.ShiftItemName,
                TraceStates = t.Key.TraceStates,
                Qualified = t.Key.Qualified,
                DetailItems = t.OrderBy(s => s.EntryTime).ToList(),
                Length = t.Count()
            }).ToList();

            return exporter.ExportToFile(result);
        }

        public async Task<FileDto> ExportNgParts(NgPartsRequestDto input)
        {
            var result = await this.ListNgPartsForExport(input);

            return exporter.ExportToFile(result.ToList(), input.StartTime, input.EndTime, input);
        }

        private async Task<List<NGPartsExportDto>> ListNgPartsForExport(NgPartsRequestDto input)
        {
            var query = await traceRepository.ListNgPartsForExport(input.PartNo, input.StartTime, input.EndTime);

            query = query
                .WhereIf(!input.DeviceGroupId.Equals(0), q => q.DeviceGroupId.Equals(input.DeviceGroupId))
                .WhereIf(!input.PartNo.IsNullOrEmpty(), q => q.PartNo.Contains(input.PartNo))
                .WhereIf(input.MachineId != null && input.MachineId.Count() != 0, q => input.MachineId.Contains(q.MachineId))
                .WhereIf(!input.ShiftSolutionItemId.Equals(0), q => q.ShiftSolutionItemId.Equals(input.ShiftSolutionItemId))
                .WhereIf(!input.StationCode.IsNullOrWhiteSpace(), q => q.StationCode.Contains(input.StationCode)).ToList();

            if (string.IsNullOrEmpty(input.PartNo))
            {
                query = query.WhereIf(input.StartTime.HasValue, q => q.OnlineTime >= input.StartTime).WhereIf(input.EndTime.HasValue, q => q.OnlineTime <= input.EndTime).ToList();
            }

            var result = query.OrderBy(x => input.Sorting).ToList();

            result.ForEach(r =>
            {
                r.DefectiveReasonNames = this.GetDefectReasonNameForExport(r);
            });

            return result;
        }

        private string GetDefectReasonNameForExport(NGPartsExportDto input)
        {
            string res = string.Empty;
            var defectQuery = (from pd in this.PartDefectRepository.GetAll()
                               join dpr in this.DefectivePartReasonRepository.GetAll() on new { PartId = pd.DefectivePartId, ReasonId = pd.DefectiveReasonId } equals new { PartId = dpr.PartId, ReasonId = dpr.ReasonId }
                               join dr in this.DefectiveReasonRepository.GetAll() on pd.DefectiveReasonId equals dr.Id
                               into pddr
                               from dri in pddr.DefaultIfEmpty()
                               where pd.PartNo == input.PartNo && pd.DefectiveMachineId == input.MachineId
                               select new DefectReasonName
                               {
                                   PartNo = pd.PartNo,
                                   DefectiveMachineId = pd.DefectiveMachineId,
                                   Name = dri.Name
                               }).ToList().GroupBy(x => new { x.PartNo, x.DefectiveMachineId }).Select(x => new DefectReasonExportDto
                               {
                                   PartNo = x.Key.PartNo,
                                   MachineId = x.Key.DefectiveMachineId,
                                   DefectiveReasonNames = x.Select(g => g.Name)
                               }).ToList();

            if (defectQuery.Any())
            {
                res = string.Join(",", defectQuery.FirstOrDefault().DefectiveReasonNames);
            }
            return res;

        }

        private async Task<Tuple<List<string>, List<string>>> GetUnionTables(DateTime? startTime, DateTime? endTime, string partNo)
        {
            var traceCatalogsTables = new List<string>();
            var traceFlowRecordTables = new List<string>();

            //如果有输入工件序列号条件，需要忽略时间条件，遍历全部的表
            if (!string.IsNullOrEmpty(partNo))
            {
                traceCatalogsTables = await this.archiveEntryRepository.GetAll()
                   .Where(s => s.TargetTable == "TraceCatalogs")
                   .GroupBy(s => s.ArchivedTable, s => s.ArchivedTable).Select(s => s.Key).ToListAsync();
            }
            else if (startTime.HasValue && endTime.HasValue)
            {
                traceCatalogsTables = (await this.archiveEntryRepository.GetAll()
               .Where(s => s.TargetTable == "TraceCatalogs").ToListAsync())
               .Where(s => startTime <= Convert.ToDateTime(s.ArchiveValue).Date && Convert.ToDateTime(s.ArchiveValue).Date <= endTime)
               .GroupBy(s => s.ArchivedTable, s => s.ArchivedTable).Select(s => s.Key).ToList();
            }
            else if (startTime.HasValue && !endTime.HasValue)
            {
                traceCatalogsTables = (await this.archiveEntryRepository.GetAll()
               .Where(s => s.TargetTable == "TraceCatalogs").ToListAsync())
               .Where(s => startTime <= Convert.ToDateTime(s.ArchiveValue).Date)
               .GroupBy(s => s.ArchivedTable, s => s.ArchivedTable).Select(s => s.Key).ToList();
            }
            else if (!startTime.HasValue && endTime.HasValue)
            {
                traceCatalogsTables = (await this.archiveEntryRepository.GetAll()
               .Where(s => s.TargetTable == "TraceCatalogs").ToListAsync())
               .Where(s => Convert.ToDateTime(s.ArchiveValue).Date <= endTime)
               .GroupBy(s => s.ArchivedTable, s => s.ArchivedTable).Select(s => s.Key).ToList();
            }

            traceFlowRecordTables = traceCatalogsTables.Select(s => s.Replace("TraceCatalogs", "TraceFlowRecords")).ToList();

            var tuple = Tuple.Create(traceCatalogsTables, traceFlowRecordTables);

            return tuple;
        }

    }

    /// <summary>
    /// 自定义比较(去重)
    /// </summary>
    public class DataRowComparer : IEqualityComparer<TraceExportDto>
    {
        public bool Equals(TraceExportDto t1, TraceExportDto t2)
        {
            return (t1.PartNo == t2.PartNo); //去重
        }

        public int GetHashCode(TraceExportDto t)
        {
            return t.ToString().GetHashCode();
        }
    }
}
