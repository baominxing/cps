namespace Wimi.BtlCore.BasicData
{
    using Abp.Application.Services.Dto;
    using Abp.Auditing;
    using Abp.Authorization;
    using Abp.AutoMapper;
    using Abp.Domain.Repositories;
    using Abp.Events.Bus;
    using Abp.Extensions;
    using Abp.IO;
    using Abp.Linq.Extensions;
    using Abp.UI;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Wimi.BtlCore;
    using Wimi.BtlCore.Authorization;
    using Wimi.BtlCore.Authorization.Roles;
    using Wimi.BtlCore.Authorization.Users;
    using Wimi.BtlCore.Authorization.Users.Dto;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.BasicData.Dto;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.BasicData.Machines.Manager;
    using Wimi.BtlCore.BasicData.MachineTypes;
    using Wimi.BtlCore.BasicData.StateInfos;
    using Wimi.BtlCore.Common;
    using Wimi.BtlCore.CommonEnums;
    using Wimi.BtlCore.Configuration;
    using Wimi.BtlCore.Dmps;
    using Wimi.BtlCore.Dto;
    using Wimi.BtlCore.Machines.Mongo;
    using Wimi.BtlCore.Storage;
    using Wimi.BtlCore.BasicData.Exporting;
    using PrimS.Telnet;
    using Microsoft.AspNetCore.Mvc;

    //[AbpAuthorize(PermissionNames.Pages_BasicData)]
    public class BasicDataAppService : BtlCoreAppServiceBase, IBasicDataAppService
    {
        // 1M (1024*1024)
        private const int OneM = 1048576;
        private const string DefaultGuid = "00000000-0000-0000-0000-000000000000";

        private readonly IBtlFolders appFolders;
        private readonly IBinaryObjectManager binaryObjectManager;

        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IRepository<MachineGatherParam, long> machineGatherParamRepository;
        private readonly IExporter exporter;
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<DeviceGroup> deviceGroupRepository;
        private readonly IRepository<MachineType> machineTypeRepository;
        private readonly IRepository<Role> roleRepository;
        private readonly IRepository<StateInfo> statusInfoRepository;
        private readonly IUserAppService userAppService;
        private readonly IRepository<MachineDriver> machineDriveRepository;
        private readonly IRepository<User, long> useRepository;
        private readonly IMachineManager machineManager;
        private readonly IEventBus enventBus;
        private readonly ICommonLookupAppService commonLookupAppService;
        private List<string> gaugeParameters = new List<string>();
        private readonly MongoMachineManager mongoMachineManager;
        private readonly IMachineTypeManager machineTypeManager;
        private readonly IStateInfoManager stateInfoManager;
        private readonly IRepository<MachineVariable,Guid> machineVariableRepository;
        private readonly IRepository<MachineNetConfig> machineNetConfigRepository;
        private readonly IRepository<Dmp> dmpRepository;

        public BasicDataAppService(
            IRepository<Machine> machineRepository,
            IRepository<MachineGatherParam, long> machineGatherParamRepository,
            IBinaryObjectManager binaryObjectManager,
            IBtlFolders appFolders,
            IRepository<StateInfo> statusInfoRepository,
            IUserAppService userAppService,
            IRepository<User, long> useRepository,
            IRepository<Role> roleRepository,
            IRepository<MachineType> machineTypeRepository,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            IExporter exporter,
            IMachineManager machineManager,
            ICommonLookupAppService commonLookupAppService,
            IRepository<DeviceGroup> deviceGroupRepository,
            IRepository<MachineDriver> machineDriveRepository,
            IEventBus enventBus, 
            MongoMachineManager mongoMachineManager,
            IMachineTypeManager machineTypeManager,
            IStateInfoManager stateInfoManager,
            IRepository<MachineVariable, Guid> machineVariableRepository,
            IRepository<MachineNetConfig> machineNetConfigRepository,
            IRepository<Dmp> dmpRepository)
        {
            this.machineRepository = machineRepository;
            this.machineGatherParamRepository = machineGatherParamRepository;
            this.binaryObjectManager = binaryObjectManager;
            this.appFolders = appFolders;
            this.statusInfoRepository = statusInfoRepository;
            this.machineGatherParamRepository = machineGatherParamRepository;
            this.userAppService = userAppService;
            this.useRepository = useRepository;
            this.roleRepository = roleRepository;
            this.machineTypeRepository = machineTypeRepository;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.exporter = exporter;
            this.machineManager = machineManager;
            this.commonLookupAppService = commonLookupAppService;
            this.deviceGroupRepository = deviceGroupRepository;
            this.machineDriveRepository = machineDriveRepository;
            this.enventBus = enventBus;
            this.mongoMachineManager = mongoMachineManager;
            this.machineTypeManager = machineTypeManager;
            this.stateInfoManager = stateInfoManager;
            this.machineVariableRepository = machineVariableRepository;
            this.machineNetConfigRepository = machineNetConfigRepository;
            this.dmpRepository = dmpRepository;
        }

        public async Task<MachineDto> AddOrUpdateMachineInRdbms(MachineSettingListDto input)
        {
            // 设备编号必须唯一
            var existMachine = !input.Id.HasValue && await this.machineRepository.CountAsync(m => m.Code == input.Code) > 0;
            if (existMachine)
                throw new UserFriendlyException(this.L("MachineCodeHasExist"));

            var machine = ObjectMapper.Map<Machine>(input);
            var imageId = await this.SaveMachineImage(machine);
            machine.ImageId = imageId;
            if (input.Id.HasValue)
            {
                var machineSetting = this.machineRepository.Get(input.Id.Value);
                if (machineSetting == null)
                    return ObjectMapper.Map<MachineDto>(machine);

                input.ImageId = imageId ?? machineSetting.ImageId;
                //input.MapTo(machineSetting);
                ObjectMapper.Map(input, machineSetting);
            }
            else
            {
                machine.SetDmpMachineId();
                await this.machineRepository.InsertOrUpdateAsync(machine);
                await this.CurrentUnitOfWork.SaveChangesAsync();
            }

            return ObjectMapper.Map<MachineDto>(machine);
        }

        [AbpAuthorize(PermissionNames.Pages_BasicData_MachineSetting_Edit, PermissionNames.Pages_BasicData_MachineSetting_Create)]
        public async Task BatchSave()
        {
            var list = ObjectMapper.Map<List<MachineSettingListDto>>(this.machineRepository.GetAll().ToList());

            foreach (var input in list)
            {
                var machine = await this.AddOrUpdateMachineInRdbms(input);
                var machineDrive = new MachineDriver() { MachineId = machine.Id, DmpMachineId = machine.DmpMachineId, Enable = false };

                var hasmachineDriver = this.machineDriveRepository.GetAll().Any(m => m.MachineId == machine.Id);
                if (!hasmachineDriver)
                    await this.machineDriveRepository.InsertAsync(machineDrive);
                this.CreateOrEditMongoMachineInfo(ObjectMapper.Map<Machine>(machine));
            }
        }

        /// <summary>
        ///     新增或编辑设备设定
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(
            PermissionNames.Pages_BasicData_MachineSetting_Edit,
            PermissionNames.Pages_BasicData_MachineSetting_Create)]
        public async Task AddOrUpdateMachineSetting(MachineSettingListDto input)
        {
            var machine = await this.AddOrUpdateMachineInRdbms(input);
            var machineDrive = new MachineDriver() { MachineId = machine.Id, DmpMachineId = machine.DmpMachineId, Enable = false };

            var hasmachineDriver = this.machineDriveRepository.GetAll().Any(m => m.MachineId == machine.Id);
            if (!hasmachineDriver)
                await this.machineDriveRepository.InsertAsync(machineDrive);
              this.CreateOrEditMongoMachineInfo(ObjectMapper.Map<Machine>(machine));
        }

        public async Task AddOrUpdateMachineType(MachineTypeDto input)
        {
            var nameIsExist = await this.machineTypeRepository.GetAll().WhereIf(input.Id != 0, s => s.Id != input.Id).AnyAsync(m => m.Name == input.Name);

            if (input.Id == 0)
            {
                if (nameIsExist)
                {
                    throw new UserFriendlyException(this.L("MachineTypeHasExist"));
                }

                var toInsert = ObjectMapper.Map<MachineType>(input);

                await this.machineTypeRepository.InsertAsync(toInsert);
            }
            else
            {
                if (nameIsExist)
                {
                    throw new UserFriendlyException(this.L("MachineTypeHasExistCannotUpdate"));
                }

                var toUpdate = await this.machineTypeRepository.GetAsync(input.Id);
                //input.MapTo(toUpdate);
                ObjectMapper.Map(input, toUpdate);

                await this.machineTypeRepository.UpdateAsync(toUpdate);
            }
        }
        /// <summary>
        /// 参数复制
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CopyParameterToMachines(GatherParameterCopyInputDto input)
        {
           
            if (input.MachineIds.Contains(input.Id))
            {
                input.MachineIds.Remove(input.Id);
            }
            if (input.MachineIds.Count > 0)
            {
                var dic = new Dictionary<string, MachineGatherParam>();
               var currentParams=await machineGatherParamRepository.GetAll().Where(p=>p.MachineId==input.Id).ToListAsync();
                foreach (var param in currentParams)
                {
                    if (!dic.Keys.Contains(param.Code))
                    {
                        dic[param.Code] = param;
                    }
                }
                var copyToParams = await machineGatherParamRepository.GetAll().Where(q => input.MachineIds.Contains(q.MachineId)).ToListAsync();
                foreach (var p in copyToParams)
                {
                    if (dic.Keys.Contains(p.Code))
                    {
                        var newParam = dic[p.Code];
                        p.DataType = newParam.DataType;
                        p.DisplayStyle = newParam.DisplayStyle;
                        p.Hexcode = newParam.Hexcode;
                        p.IsShowForStatus = newParam.IsShowForStatus;
                        p.IsShowForVisual = newParam.IsShowForVisual;
                        p.IsShowForParam = newParam.IsShowForParam;
                        p.Unit = newParam.Unit;
                        await machineGatherParamRepository.UpdateAsync(p);
                    }
                }
            }
           
        }

        public void CheckImageSize(int size)
        {
            if (size > OneM) throw new UserFriendlyException(this.L("ResizedProfilePicture_Warn_SizeLimit"));
        }

        /// <summary>
        ///     编辑
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreatOrUpdateStateInfo(CreateOrUpdateStateInfoDto input)
        {
            input.Code = input.Code.Trim();
            input.DisplayName = input.DisplayName.Trim();
            if (!input.Id.HasValue)
            {
                // 新建
                await this.CheckCode(input.Code);
                await this.CheckName(input.DisplayName);
                var createEntity = ObjectMapper.Map<StateInfo>(input);
                await this.statusInfoRepository.InsertAsync(createEntity);
                return;
            }

            // 编辑
            var entity = await this.statusInfoRepository.GetAsync(input.Id.Value);
            if (entity.DisplayName != input.DisplayName) await this.CheckName(input.DisplayName);

            if (entity.IsStatic) input.IsStatic = true;

            //input.MapTo(entity);
            ObjectMapper.Map(input, entity);
            await this.statusInfoRepository.UpdateAsync(entity);
        }

        /// <summary>
        ///     按照主键Id删除设备设定
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.Pages_BasicData_MachineSetting_Delete)]
        public void DeleteMachine(EntityDto input)
        {
            var machine = this.machineRepository.Get(input.Id);

            if (machine == null) return;

            this.enventBus.Trigger(new MachineEventData(machine));
        }

        public async Task DeleteMachineGatherParam(EntityDto input)
        {
            var target = await this.machineGatherParamRepository.GetAsync(input.Id);
            if (target != null) await this.machineGatherParamRepository.DeleteAsync(input.Id);
        }

        public async Task DeleteMachineType(EntityDto input)
        {
            //判断是否有设备正在使用该设备类型
            this.machineTypeManager.CheckIsInMachine(input.Id); 

            await this.machineTypeRepository.DeleteAsync(input.Id);
        }

        // 删除
        public async Task DeleteStateInfo(NullableIdDto input)
        {
            if (input.Id.HasValue)
            {
                var entity = await this.statusInfoRepository.FirstOrDefaultAsync(input.Id.Value);

                if (entity == null || entity.IsStatic) return;

                //判断是否能删除该状态
                if (this.stateInfoManager.IsInUsing(entity))
                {
                    throw new UserFriendlyException(this.L("StatusReasonIsInUse"));
                }

                await this.statusInfoRepository.DeleteAsync(input.Id.Value);
            }
        }

        /// <summary>
        ///     停用和启用设备
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.Pages_BasicData_MachineSetting_Edit)]
        public void EnableOrDisEnableMachine(NullableIdDto input)
        {
            if (input.Id.HasValue)
            {
                try
                {
                    var machine = this.machineRepository.Get(input.Id.Value);
                    machine.IsActive = !machine.IsActive; 
                    mongoMachineManager.InsertOrUpdateMongoMachine(machine);
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException(this.L("MachineUpFailedReason{0}", ex.Message));
                }
            }
        }

        [AbpAuthorize(PermissionNames.Pages_BasicData_ImportData)]
        [HttpPost]
        public List<ImportDataTablesColumnsDto> GetImportDataTablesColumns(ImportDataInputDto input)
        {
            EnumImportTypes inputType;
            if (!Enum.TryParse(input.Type, out inputType)) inputType = EnumImportTypes.Users;

            List<ImportDataTablesColumnsDto> list;

            switch (inputType)
            {
                case EnumImportTypes.Users:
                    list = this.GetImportUsersColumns();
                    break;
                default:
                    list = this.GetImportMachinesColumns();
                    break;
            }

            return list;
        }
        [HttpPost]
        public async Task<MachineGatherParamDto> GetMachineGatherParamForEdit(NullableIdDto input)
        {
            var returnValue = new MachineGatherParamDto();

            if (input.Id.HasValue)
            {
                var query = await this.machineGatherParamRepository.GetAsync(input.Id.Value);
                returnValue = ObjectMapper.Map<MachineGatherParamDto>(query);
            }
            return returnValue;

            //return input.Id.HasValue
            //           ? (await this.machineGatherParamRepository.GetAsync(input.Id.Value))
            //           .MapTo<MachineGatherParamDto>()
            //           : new MachineGatherParamDto();
        }

        /// <summary>
        ///     设备设定，table获取数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PagedResultDto<MachineSettingListDto>> GetMachineSetting(MachineSettingInputDto input)
        {
            var query = from m in this.machineRepository.GetAll()
                        join mt in this.machineTypeRepository.GetAll() on m.MachineTypeId equals mt.Id
                        select new MachineSettingListDto
                        {
                            Id = m.Id,
                            Code = m.Code,
                            CreationTime = m.CreationTime,
                            CreatorUserId = m.CreatorUserId,
                            Desc = m.Desc,
                            ImageId = m.ImageId,
                            IsActive = m.IsActive,
                            LastModificationTime = m.LastModificationTime,
                            LastModifierUserId = m.LastModifierUserId,
                            MachineTypeId = m.MachineTypeId,
                            MachineTypeName = mt.Name,
                            Name = m.Name,
                            SortSeq = m.SortSeq
                        };


            query = query.WhereIf(!input.Search.Value.IsNullOrWhiteSpace(), m => m.Name.Contains(input.Search.Value) || m.Code.Contains(input.Search.Value));
            
            var machine = await query.OrderBy(input.Sorting).AsNoTracking().PageBy(input).ToListAsync();
            var machineCount = await query.CountAsync();
            return new DatatablesPagedResultOutput<MachineSettingListDto>(
                       machineCount,
                      ObjectMapper.Map<List<MachineSettingListDto>>(machine),
                       machineCount)
            {
                Draw = input.Draw
            };
        }

        [HttpPost]
        public MachineSettingListDto GetMachineSettingById(NullableIdDto input)
        {
            var settingDto = new MachineSettingListDto();
            if (input.Id.HasValue)
            {
                var machineSetting = this.machineRepository.Get(input.Id.Value);
                var netConfig = this.machineNetConfigRepository.FirstOrDefault(m => m.MachineCode.Equals(machineSetting.Code));

                if (machineSetting != null)
                {
                    ObjectMapper.Map(machineSetting, settingDto);
                }

                if(netConfig != null)
                {
                    settingDto.IpAddress = netConfig.IpAddress;
                    settingDto.TcpPort = netConfig.TcpPort;
                }
            }

            return settingDto;
        }

        /// <summary>
        /// 测试设备IP是否可以ping通
        /// </summary>
        /// <param name="input"></param>
        public void PingTestForMachine(TestPingOrTelnetDto input)
        {
            var targetMachine = this.machineRepository.FirstOrDefault(m => m.Id == input.MachineId);
            Ping p1 = new Ping();
            PingReply reply = p1.Send(input.IpAddress); //发送主机名或Ip地址
            if (reply.Status == IPStatus.Success)
            { 
                return;
            }
            else if (reply.Status == IPStatus.TimedOut)
            {
                throw new UserFriendlyException($"设备【{targetMachine.Name}】Ping连接超时");
            }
            else
            {
                throw new UserFriendlyException($"设备【{targetMachine.Name}】Ping连接失败");
            }

        }

        /// <summary>
        /// 测试设备端口是否可以telnet连通
        /// </summary>
        /// <param name="input"></param>
        public void TelnetTestForMachine(TestPingOrTelnetDto input)
        {
            var targetMachine = this.machineRepository.FirstOrDefault(m => m.Id == input.MachineId);

            try
            {
                using (Client client = new Client(input.IpAddress, input.TcpPort.Value, new System.Threading.CancellationToken()))
                {
                    if (client.IsConnected)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException($"设备【{targetMachine.Name}】 Telnet连接出现错误,错误信息:"
                        + ex.Message);
            }
        }

        /// <summary>
        /// 测试DMP是否正在启动
        /// </summary>
        public void TelnetTesFortDMP()
        {
            var dmpLastStartConfig = this.dmpRepository.GetAll().OrderByDescending(d => d.CreationTime).FirstOrDefault();

            if (dmpLastStartConfig != null)
            {
                try
                {
                    using (Client client = new Client(dmpLastStartConfig.IpAdress, Convert.ToInt32(dmpLastStartConfig.WebPort), new System.Threading.CancellationToken()))
                    {
                        if (client.IsConnected)
                        {

                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException($"DMP Telnet连接出现错误,错误信息:"
                            + ex.Message);
                }
            }
            else
            {
                throw new UserFriendlyException($"请先启动DMP");
            }
        }

        public IEnumerable<NameValueDto<int>> ListMachineInDeviceGroup(int deviceGroupId)
        {
            var machines = (from mdg in this.machineDeviceGroupRepository.GetAll()
                join m in this.machineRepository.GetAll() on mdg.MachineId equals m.Id
                where mdg.DeviceGroupId == deviceGroupId && m.IsActive
                select new NameValueDto<int>
                {
                    Value= m.Id,
                   Name = m.Name
                }).ToList();
            return machines;
        }

        [HttpPost]
        public FileDto GetMachinesToExcel()
        {
            var list = this.machineRepository.GetAll()
                .Select(s => new MachineSettingListDto { Id = s.Id, Name = s.Name })
                .ToList();

            return this.exporter.ExportToFile(list);
        }

        [HttpPost]
        public async Task<FileDto> GetMachinesToXML()
        {
            var deviceGroupsAndMachinesWithPermissions = await this.commonLookupAppService.GetDeviceGroupAndMachineWithPermissions();

            var grantedGroupIdsMapping = this.GetGrantedGroupIdsMapping(deviceGroupsAndMachinesWithPermissions.GrantedGroupIds);

            var machineList = (from mdg in this.machineDeviceGroupRepository.GetAll()
                               join m in this.machineRepository.GetAll() on mdg.MachineId equals m.Id
                               join mg in this.deviceGroupRepository.GetAll() on mdg.DeviceGroupId equals mg.Id
                               where deviceGroupsAndMachinesWithPermissions.GrantedGroupIds.Contains(mdg.DeviceGroupId)
                               select new ExportMachineDto()
                               {
                                   MachineId = Guid.NewGuid().ToString(),
                                   MachineName = m.Code,
                                   GroupId = mg.Id.ToString(),
                                   GroupCode = mg.Code,
                                   GroupName = mg.DisplayName,
                                   ParentGroupId = mg.ParentId.ToString()
                               }).ToList().Select(s => new ExportMachineDto()
                               {
                                   MachineId = s.MachineId,
                                   MachineName = s.MachineName,
                                   GroupId = grantedGroupIdsMapping.ContainsKey(s.GroupId) ? grantedGroupIdsMapping[s.GroupId] : DefaultGuid,
                                   GroupCode = s.GroupCode,
                                   GroupName = s.GroupName,
                                   ParentGroupId = grantedGroupIdsMapping.ContainsKey(s.ParentGroupId) ? grantedGroupIdsMapping[s.ParentGroupId] : DefaultGuid
                               }).ToList();

            var groupList = (from mg in this.deviceGroupRepository.GetAll()
                             where deviceGroupsAndMachinesWithPermissions.GrantedGroupIds.Contains(mg.Id)
                             group new { GroupId = mg.ParentId, Id = mg.Id, GroupCode = mg.Code, Name = mg.DisplayName } by new { GroupId = mg.ParentId, Id = mg.Id, GroupCode = mg.Code, Name = mg.DisplayName } into mgg
                             select new ExportMachineDto()
                             {
                                 GroupId = mgg.Key.Id.ToString(),
                                 GroupCode = mgg.Key.GroupCode,
                                 GroupName = mgg.Key.Name,
                                 ParentGroupId = mgg.Key.GroupId.ToString()
                             }).ToList().Select(s => new ExportMachineDto()
                             {
                                 GroupId = grantedGroupIdsMapping.ContainsKey(s.GroupId) ? grantedGroupIdsMapping[s.GroupId] : DefaultGuid,
                                 GroupCode = s.GroupCode,
                                 GroupName = s.GroupName,
                                 ParentGroupId = grantedGroupIdsMapping.ContainsKey(s.ParentGroupId) ? grantedGroupIdsMapping[s.ParentGroupId] : DefaultGuid
                             }).OrderBy(s => s.GroupCode).ToList();


            return this.exporter.ExportMachineToXML(machineList, groupList);
        }

        private Dictionary<string, string> GetGrantedGroupIdsMapping(List<int> grantedGroupIds)
        {
            var returnValue = new Dictionary<string, string>();

            foreach (var item in grantedGroupIds)
            {
                returnValue.Add(item.ToString(), Guid.NewGuid().ToString());
            }

            return returnValue;
        }

        [HttpPost]
        public string GetMachineImagePath(EntityDto<Guid?> input)
        {
            return this.machineManager.GetMachineImagePath(input);
        }

        [HttpPost]
        public async Task<PagedResultDto<MachineTypeDto>> GetMachineTypeList(MachineTypeInputDto input)
        {
            var query = this.machineTypeRepository.GetAll()
                .WhereIf(
                    !input.Search.Value.IsNullOrWhiteSpace(),
                    g => g.Name.Contains(input.Search.Value) || g.Desc.Contains(input.Search.Value));

            var result = await query.OrderBy(input.Sorting).AsNoTracking().PageBy(input).ToListAsync();
            var resultCount = await query.CountAsync();
            return new DatatablesPagedResultOutput<MachineTypeDto>(
                       resultCount,
                      ObjectMapper.Map<List<MachineTypeDto>>(result),
                       resultCount)
            {
                Draw = input.Draw
            };
        }

        public async Task<IEnumerable<NameValueDto<int>>> ListMachineTypes()
        {
            return await this.machineTypeRepository.GetAll().Select(n =>new NameValueDto<int>()
            {
                Name=n.Name,
                Value = n.Id
            }).ToListAsync();
        }

        [DisableAuditing]
        [HttpPost]
        public async Task<IEnumerable<StateInfoOutputDto>> GetStateForVisual()
        {
            var result = await this.statusInfoRepository.GetAll().Where(s => s.Type == EnumMachineStateType.State)
                             .ToListAsync();

            return ObjectMapper.Map<IEnumerable<StateInfoOutputDto>>(result);
        }

        [HttpPost]
        /// <summary>
        ///     获取状态维护列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PagedResultDto<CreateOrUpdateStateInfoDto>> GetStateInfoList(GetStateInfoListDto input)
        {
            var query = this.statusInfoRepository.GetAll()
                .WhereIf(
                    !input.Search.Value.IsNullOrWhiteSpace(),
                    s => s.Code.Contains(input.Search.Value) || s.DisplayName.Contains(input.Search.Value));

            var returnValue = await query.OrderBy(input.Sorting).AsNoTracking().PageBy(input).ToListAsync();
            var count = await query.CountAsync();

            return new DatatablesPagedResultOutput<CreateOrUpdateStateInfoDto>(
                       count,
                      ObjectMapper .Map<List<CreateOrUpdateStateInfoDto>>(returnValue),
                       count)
            {
                Draw = input.Draw
            };
        }

        public async Task<MachineGatherParamDto> UpdateGatherParams(UpdateGatherParamsInputDto input)
        {
            if (Math.Abs(input.Min) > 0 || Math.Abs(input.Max) > 0)
            {
                this.gaugeParameters.Clear();

                var gauges = SettingManager.GetSettingValue(AppSettings.MachineParameter.GaugeParameters).Split(',');

                this.gaugeParameters = (from e in this.machineGatherParamRepository.GetAll()
                                        where e.MachineId == input.MachineId
                                              && gauges.Contains(e.Code)
                                        select e.Name).ToList();

                if (!this.gaugeParameters.Contains(input.Name)) throw new UserFriendlyException(this.L("ParameterTypeDoesNotSupport"));
            }

            MachineGatherParam entity = null;
            if (input.Id > 0)
            {
                entity = await this.machineGatherParamRepository.GetAsync(input.Id);
                //input.MapTo(entity);
                ObjectMapper.Map(input, entity);

                entity.DefaultSetting();
            }

            return ObjectMapper.Map<MachineGatherParamDto>(entity);
        }

        /// <summary>
        ///     switch  是否显示在看板上
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateMachineGatherParamPriorShow(EntityDto input)
        {
            var target = await this.machineGatherParamRepository.GetAsync(input.Id);
            target.IsShowForVisual = !target.IsShowForVisual;
        }

        /// <summary>
        ///     switch 是否显示在实时状态上
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateMachineGatherParamShowState(EntityDto input)
        {
            var target = await this.machineGatherParamRepository.GetAsync(input.Id);
            target.IsShowForStatus = !target.IsShowForStatus;
        }

        /// <summary>
        ///     switch 是否显示在运行参数上
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateMachineGatherParamShowParam(EntityDto input)
        {
            var target = await this.machineGatherParamRepository.GetAsync(input.Id);
            target.IsShowForParam = !target.IsShowForParam;
        }

        /// <summary>
        ///     数据导入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<ImportDataOutputDto> ValidateImportData(ImportDataOutputDto input)
        {
            ImportDataOutputDto returnValue;
            switch (input.Type)
            {
                case EnumImportTypes.Users:
                    returnValue = await this.ImportUsersData(input.UsersList);
                    break;
                case EnumImportTypes.Machines:
                    returnValue = await this.ImportMachinesData(input.MachinesList);
                    break;
                default:
                    returnValue = await this.ImportGatherParamsData(input.GatherParamsList);
                    break;
            }

            return returnValue;
        }

        private async Task CheckCode(string code)
        {
            if (await this.statusInfoRepository.GetAll().AnyAsync(m => m.Code == code))
                throw new UserFriendlyException(this.L("StateCodingMustBeUnique"));
        }

        private async Task<string> CheckMachinesDto(ImportMachinesOutputDto machinesDto)
        {
            var machineType = this.machineTypeRepository.GetAll().Select(p => p.Name).ToList();
            var errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(machinesDto.Code))
            {
                return this.L("MachineCodeCannotBeNull");
            }

            if (string.IsNullOrWhiteSpace(machinesDto.Name)) errorMessage += ";" + this.L("MachineNameCannotBeNull");

            if (string.IsNullOrWhiteSpace(machinesDto.Type))
            {
                errorMessage += ";" + this.L("MachineTypeCannotBeNull");
            }
            else
            {
                if (!machineType.Contains(machinesDto.Type)) errorMessage += ";" + this.L("MachineTypeNotExist");
            }

            if (await this.machineRepository.CountAsync(m => m.Code.ToLower().Equals(machinesDto.Code.ToLower())) > 0)
                errorMessage += ";" + this.L("DoNotImportMachineAgain");

            return errorMessage.Trim(';');
        }

        private async Task CheckName(string name)
        {
            if (await this.statusInfoRepository.GetAll().AnyAsync(m => m.DisplayName == name))
                throw new UserFriendlyException(this.L("StatusNameMustBeUnique"));
        }

        private async Task<string> CheckUsersDto(ImportUsersOutputDto userDto)
        {
            var errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(userDto.Name)) errorMessage += ";" + this.L("NameCannotBeNull");

            if (string.IsNullOrWhiteSpace(userDto.UserName)) errorMessage += ";" + this.L("UserNameCannotBeNull");

            if (string.IsNullOrWhiteSpace(userDto.Password)) errorMessage += ";" + this.L("PasswordCannotBeNull");
            else if (userDto.Password.Length < 6) errorMessage += ";" + this.L("PasswordMinLengh");

            if (string.IsNullOrWhiteSpace(userDto.RolesName)) errorMessage += ";" + this.L("RoleNameCannotBeNull");
            else if (await this.roleRepository.CountAsync(r => r.DisplayName.ToLower().Equals(userDto.RolesName)) <= 0)
                errorMessage += ";" + this.L("RoleNameDoesNotExist");

            var regexEmail = "\\w{1,}@\\w{1,}\\.\\w{1,}";
            var options = RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase;
            var regEmail = new Regex(regexEmail, options);

            if (await this.useRepository.CountAsync(u => u.UserName.ToLower().Equals(userDto.UserName)) > 0)
                errorMessage += ";" + this.L("UserNameHasExist");

            return errorMessage.Trim(';');
        }

        private async Task<string> CheckGatherParamsDto(ImportGatherParamsOutputDto paramDto)
        {
            var errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(paramDto.MachineId)) errorMessage += ";"+this.L("MachineIdCanNotBeNull");
            else
            {
                if (!Regex.IsMatch(paramDto.MachineId, @"^[0-9]*[1-9][0-9]*$")) errorMessage += ";"+L("MachineIdMustBePositiveInteger");
                else
                {
                    var machineId = Convert.ToInt32(paramDto.MachineId);
                    if(!await this.machineRepository.GetAll().AnyAsync(x=>x.Id == machineId))
                        errorMessage += ";"+L("MachineIdNotExist");
                }
            }

            if (string.IsNullOrWhiteSpace(paramDto.Code)) errorMessage += ";"+L("VariableNameCanNotBeNull");

            if (string.IsNullOrWhiteSpace(paramDto.DeviceAddress)) errorMessage += ";"+L("DataAddressCanNotBeNull");

            if (string.IsNullOrWhiteSpace(paramDto.DataType)) errorMessage += ";"+L("DataTypeNotRight");
            else
            {
                if (!string.IsNullOrWhiteSpace(paramDto.DefaultValue))
                {
                    var defaultValueErrorMessage = this.CheckGatherParamsDefaultValue(paramDto);
                    if (!string.IsNullOrWhiteSpace(defaultValueErrorMessage)) errorMessage += $";{defaultValueErrorMessage}";
                }
            }

            if (paramDto.Access == 0) errorMessage += ";"+L("AccessPermissionsNotRight");

            if (!string.IsNullOrWhiteSpace(paramDto.ValueFactor) && !Regex.IsMatch(paramDto.ValueFactor, @"^\d+(\.\d+)?$"))
                errorMessage += ";"+L("DataRateIncorrectFormat");

            return errorMessage.Trim(';');
        }

        private void CreateOrEditMongoMachineInfo(Machine machine)
        {
            try
            {
                mongoMachineManager.InsertOrUpdateMongoMachine(machine);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(this.L("MongoDbException")+e.ToString());
            }
        }

        private List<ImportDataTablesColumnsDto> GetImportMachinesColumns()
        {
            return typeof(ImportMachinesOutputDto).GetProperties()
                .Select(
                    p => new ImportDataTablesColumnsDto
                    {
                        Title = this.GetAttributeNameArgumentTitle(p),
                        Data = p.Name.ToCamelCase().ToString()
                    })
                .ToList();
        }

        private List<ImportDataTablesColumnsDto> GetImportUsersColumns()
        {
            return typeof(ImportUsersOutputDto).GetProperties()
                .Select(
                    p => new ImportDataTablesColumnsDto
                    {
                        Title = this.GetAttributeNameArgumentTitle(p),
                        Data = p.Name.ToCamelCase().ToString()
                    })
                .ToList();
        }

        private string GetAttributeNameArgumentTitle(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null
                || propertyInfo.CustomAttributes == null)
            {
                return string.Empty;
            }

            var namedArguments = propertyInfo.CustomAttributes.First().NamedArguments;
            if (namedArguments == null)
            {
                return string.Empty;
            }

            return namedArguments.First().TypedValue.Value.ToString();
        }

        private async Task<ImportDataOutputDto> ImportMachinesData(List<ImportMachinesOutputDto> input)
        {
            foreach (var machineDto in input)
            {
                // 验证数据
                machineDto.ErrorMessage = await this.CheckMachinesDto(machineDto);

                // 将验证通过的数据导入对应的表中
                if (string.IsNullOrWhiteSpace(machineDto.ErrorMessage))
                    try
                    {
                        await this.InsertMachineTable(machineDto);
                        machineDto.ErrorMessage = "Success";
                    }
                    catch (Exception ex)
                    {
                        machineDto.ErrorMessage = ex.Message;
                    }
            }

            return new ImportDataOutputDto { MachinesList = input };
        }

        private async Task<ImportDataOutputDto> ImportUsersData(List<ImportUsersOutputDto> input)
        {
            foreach (var userDto in input)
            {
                // 验证数据
                userDto.ErrorMessage = await this.CheckUsersDto(userDto);

                // 将验证通过的数据导入对应的表中
                if (string.IsNullOrWhiteSpace(userDto.ErrorMessage))
                    try
                    {
                        await this.InsertUserTable(userDto);
                        userDto.ErrorMessage = "Success";
                    }
                    catch (Exception ex)
                    {
                        userDto.ErrorMessage = ex.Message;
                    }
            }

            return new ImportDataOutputDto { UsersList = input };
        }

        private async Task<ImportDataOutputDto> ImportGatherParamsData(List<ImportGatherParamsOutputDto> input)
        {
            foreach (var paramsDto in input)
            {
                // 验证数据
                paramsDto.ErrorMessage = await this.CheckGatherParamsDto(paramsDto);

                // 将验证通过的数据导入对应的表中
                if (string.IsNullOrWhiteSpace(paramsDto.ErrorMessage))
                    try
                    {
                        await this.InsertGatherParamsTable(paramsDto);
                        paramsDto.ErrorMessage = "Success";
                    }
                    catch (Exception ex)
                    {
                        paramsDto.ErrorMessage = ex.Message;
                    }
            }

            return new ImportDataOutputDto { GatherParamsList = input };
        }

        private async Task InsertMachineTable(ImportMachinesOutputDto machinesDto)
        {
            var machineTypeId = this.machineTypeRepository.FirstOrDefault(p => p.Name == machinesDto.Type).Id;
            var machine = ObjectMapper.Map<Machine>(machinesDto);
            machine.SetDmpMachineId();
            machine.MachineTypeId = machineTypeId;
            machine.SortSeq = 1;
            await this.machineRepository.InsertAsync(machine);
            await this.CurrentUnitOfWork.SaveChangesAsync();
            var machineDrive = new MachineDriver() { MachineId = machine.Id, DmpMachineId = machine.DmpMachineId, Enable = false };
            var hasmachineDriver = this.machineDriveRepository.GetAll().Any(m => m.MachineId == machine.Id);
            if (!hasmachineDriver)
                await this.machineDriveRepository.InsertAsync(machineDrive);
            this.CreateOrEditMongoMachineInfo(machine);

        }

        private async Task InsertUserTable(ImportUsersOutputDto userDto)
        {
            var userEditDto = ObjectMapper.Map<UserEditDto>(userDto);

            var user = new CreateOrUpdateUserInputDto
            {
                User = userEditDto,
                AssignedRoleNames = new[] { userDto.RolesName },
                SendActivationEmail = false
            };
            await this.userAppService.CreateOrUpdateUser(user);
            await this.CurrentUnitOfWork.SaveChangesAsync();
        }

        private async Task InsertGatherParamsTable(ImportGatherParamsOutputDto paramDto)
        {
            var machineId = Convert.ToInt32(paramDto.MachineId);
            var machine = await this.machineRepository.GetAsync(machineId);

            //插入MachineVariable表
            var machineVariableDto = ObjectMapper.Map<MachineVariable>(paramDto);

            machineVariableDto.DmpMachineId = machine.DmpMachineId;

            if (string.IsNullOrWhiteSpace(machineVariableDto.DefaultValue))
            {
                machineVariableDto.DefaultValue = machineVariableDto.DataType == "0" ? "False" : "0";
            }

            var machineVariable = await this.machineVariableRepository.GetAll().Where(x => x.MachineId == machine.Id)
                .OrderByDescending(x=>x.Index)
                .FirstOrDefaultAsync();
            machineVariableDto.Index = machineVariable == null ? 0 : machineVariable.Index + 1;

            await this.machineVariableRepository.InsertAsync(machineVariableDto);
            await this.CurrentUnitOfWork.SaveChangesAsync();

            //插入MachineGatherParam表
            var machineGatherParam = await this.machineGatherParamRepository.GetAll().Where(x => x.MachineId == machine.Id)
                .OrderByDescending(x => x.SortSeq)
                .FirstOrDefaultAsync();
            var machineGatherParamsDto = new MachineGatherParam
            {
                MachineId = machine.Id,
                Code = paramDto.Code,
                Name = paramDto.Description,
                MachineCode = machine.Code,
                SortSeq = machineGatherParam == null ? 0 : machineGatherParam.SortSeq + 1,
                Hexcode = "#204D74",
                Min = 0,
                Max = 0,
                IsShowForVisual = true,
                IsShowForStatus = true,
                DataType = this.ConvertToDotNetDataType(paramDto.DataType),
                MachineVariableId = machineVariableDto.Id
            };
            machineGatherParamsDto.InsertDisplayStyleSetting();
            await this.machineGatherParamRepository.InsertAsync(machineGatherParamsDto);
            await this.CurrentUnitOfWork.SaveChangesAsync();

        }

        private string ConvertToDotNetDataType(string dataType)
        {
            var convertedDataType = string.Empty;

            var originalDataType = Enum.Parse(typeof(EnumDataType), dataType).ToString();
            switch (originalDataType)
            {
                case "Bit":
                    convertedDataType = "Boolean";
                    break;
                case "Byte":
                case "Word":
                case "DWord":
                case "QWord":
                case "SByte":
                case "SWord":
                case "SDWord":
                case "SQWord":
                case "Float":
                case "Double":
                    convertedDataType = "Number";
                    break;
                case "String":
                    convertedDataType = "Text";
                    break;
                case "DateTime":
                    convertedDataType = "DataTime";
                    break;
            }

            return convertedDataType;
        }


        private string CheckGatherParamsDefaultValue(ImportGatherParamsOutputDto paramDto)
        {
            var errorMessage = string.Empty;

            var originalDataType = Enum.Parse(typeof(EnumDataType), paramDto.DataType).ToString();
            switch (originalDataType)
            {
                case "Bit":
                    if (paramDto.DefaultValue.ToLower()!= "false" && paramDto.DefaultValue.ToLower() != "true")
                    {
                        errorMessage = L("InitialValueFormatIsIncorrect");
                    }
                    break;
                case "Byte":
                case "Word":
                case "DWord":
                case "QWord":
                case "SByte":
                case "SWord":
                case "SDWord":
                case "SQWord":
                    if (!Regex.IsMatch(paramDto.DefaultValue, @"^\d+$")) errorMessage = L("InitialValueFormatIsIncorrect");
                    break;
                case "Float":
                case "Double":
                    if (!Regex.IsMatch(paramDto.DefaultValue, @"^(-?\d+)(\.\d+)?$")) errorMessage = L("InitialValueFormatIsIncorrect"); 
                    break;
            }

            return errorMessage;
        }

        private async Task<Guid?> SaveMachineImage(Machine machine)
        {
            var fileNameWithoutExtension = machine.ImageId;
            if (!machine.ImageId.HasValue) return null;

            var fileExtension = ".jpg";
            var tempProfilePicturePath = Path.Combine(
                this.appFolders.TempFileDownloadFolder,
                fileNameWithoutExtension + fileExtension);
            byte[] byteArray;

            using (var machineImageStream = new FileStream(tempProfilePicturePath, FileMode.Open,FileAccess.Read,FileShare.Read))
            {
                var tempByteArray = new byte[machineImageStream.Length];
                try
                {
                    machineImageStream.Read(tempByteArray, 0, tempByteArray.Length);
                    machineImageStream.Seek(0, SeekOrigin.Begin);
                    byteArray = tempByteArray;
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException(ex.Message);
                }
            }

            if (byteArray.Length == 0) return null;

            this.CheckImageSize(byteArray.Length);

            if (machine.Id > 0)
            {
                var machineDb = this.machineRepository.Get(machine.Id);
                if (machineDb != null && machineDb.ImageId.HasValue)
                    await this.binaryObjectManager.DeleteAsync(machineDb.ImageId.Value);
            }

            var storedFile = new BinaryObject(null, byteArray);
            await this.binaryObjectManager.SaveAsync(storedFile);
            FileHelper.DeleteIfExists(tempProfilePicturePath);

            return storedFile.Id;
        }

        public async Task<FileDto> ExportMachinesToExcel()
        {
            var query = await(from m in machineRepository.GetAll()
                              join t in machineTypeRepository.GetAll() on m.MachineTypeId equals t.Id
                              //join n in machineNetConfigRepository.GetAll() on m.Code equals n.MachineCode into n2
                              //from n22 in n2.DefaultIfEmpty()
                              orderby m.SortSeq, m.Code ascending
                              select new MachineDto()
                              {
                                  Id = m.Id,
                                  Code = m.Code,
                                  Name = m.Name,
                                  MachineTypeName = t.Name,
                                  SortSeq = m.SortSeq,
                                  IsActive = m.IsActive,
                                  //IpAddress = n22.IpAddress,
                                  //TcpPort = n22.TcpPort

                              }).ToListAsync();

            for (var n = 0; n < query.Count; n++)
            {
                var code = query[n].Code;
                var netConfig = await machineNetConfigRepository.GetAll().Where(p => p.MachineCode == code).FirstOrDefaultAsync();
                query[n].IpAddress = netConfig == null ? "" : netConfig.IpAddress;
                query[n].TcpPort = netConfig == null ? null : netConfig.TcpPort;
            }

            return exporter.ExportMachineToFile(query);
        }
        [DisableAuditing]
        public async Task<IEnumerable<StateInfoOutputDto>> GetStateAndReasonForVisual()
        {
            var result = await this.statusInfoRepository.GetAll()
                             .ToListAsync();
            var entity = new List<StateInfoOutputDto>();
            ObjectMapper.Map(result, entity);
            return entity;
        }

    }
}