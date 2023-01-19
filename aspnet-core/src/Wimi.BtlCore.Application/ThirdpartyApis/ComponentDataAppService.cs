using Wimi.BtlCore.ThirdpartyApis.Dto;
using Wimi.BtlCore.ThirdpartyApis.Interfaces;
using Abp.Authorization;
using Abp.Domain.Repositories;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines.Manager;
using Wimi.BtlCore.Machines.Mongo;
using Wimi.BtlCore.Notices;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.Archives;
using Microsoft.AspNetCore.Mvc;

namespace Wimi.BtlCore.ThirdpartyApis
{
    [AbpAllowAnonymous]
    public class ComponentDataAppService : BtlCoreDomainServiceBase, IComponentDataAppService
    {
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IRepository<Notice> noticeRepository;
        private readonly IThirdpartyApiManager thirdpartyApiManager;
        private readonly IMachineManager machineManager;
        private readonly IMachineComponentManager machineComponentManager;
        private readonly IStatisticalComponentManager statisticalComponentManager;
        private readonly IPlanComponentManager planComponentManager;
        private readonly ICutterComponentManager cutterComponentManager;
        private readonly MongoMachineManager mongoMachineManager;

        public ComponentDataAppService(
            IThirdpartyApiManager thirdpartyApiManager,
            IMachineManager machineManager,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            IRepository<Notice> noticeRepository,
            IMachineComponentManager machineComponentManager,
            IStatisticalComponentManager statisticalComponentManager,
            IPlanComponentManager planComponentManager,
            ICutterComponentManager cutterComponentManager,
            MongoMachineManager mongoMachineManager
            )
        {
            this.thirdpartyApiManager = thirdpartyApiManager;
            this.machineManager = machineManager;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.noticeRepository = noticeRepository;
            this.machineComponentManager = machineComponentManager;
            this.statisticalComponentManager = statisticalComponentManager;
            this.planComponentManager = planComponentManager;
            this.cutterComponentManager = cutterComponentManager;
            this.mongoMachineManager = mongoMachineManager;
          
        }

        public async Task<ApiResponseObject> ListRealtimeMachineInfos(RequestDto input)
        {
            if (input.Demo)
            {
                return this.thirdpartyApiManager.ListRealtimeMachineInfoDemoData(input.WorkShopCode);
            }
            var machines = this.machineManager.ListMachinesInDeviceGroup(input.WorkShopCode).ToList();
            var documents = mongoMachineManager.ListOriginalMongoMachinesBsonDocument(machines.Select(m => m.Code)).ToList();

            return await this.machineComponentManager.ListRealtimeMachineInfos(documents, machines);

        }

        public ApiResponseObject ListMachineAlarming(RequestDto input)
        {
            if (input.Demo)
            {
                return this.thirdpartyApiManager.ListMachineAlarmingDemoData(input.WorkShopCode);
            }

            var machines = this.machineManager.ListMachinesInDeviceGroup(input.WorkShopCode).ToList();
            var documents = mongoMachineManager.ListOriginalMongoMachinesBsonDocument(machines.Select(m => m.Code)).ToList();

            return  this.machineComponentManager.ListMachineAlarming(documents,machines);

        }

        public async Task<ApiResponseObject> ListPerHourYields(RequestDto input)
        {
            if (input.Demo)
            {
                return this.thirdpartyApiManager.ListPerHourYieldDemoData(input.WorkShopCode);
            }

            return await this.statisticalComponentManager.ListPerHourYields(input.WorkShopCode);

           
        }

        public async Task<ApiResponseObject> ListHourlyMachineYiled(RequestDto input)
        {
            if (input.Demo)
            {
                return this.thirdpartyApiManager.ListHourlyMachineYiledDemoData(input.WorkShopCode);
            }

            return await this.statisticalComponentManager.ListHourlyMachineYiled(input.WorkShopCode);

           
        }

        public async Task<ApiResponseObject> ListHourlyMachineYiledByShiftDay(RequestDto input)
        {
            if (input.Demo)
            {
                return this.thirdpartyApiManager.ListHourlyMachineYiledByShiftDayDemoData(input.WorkShopCode);
            }

            return await this.statisticalComponentManager.ListHourlyMachineYiledByShiftDay(input.WorkShopCode);
        }

        [HttpPost]
        public async Task<ApiResponseObject> ListNotices([FromForm]RequestDto input)
        {
            if (input.Demo)
            {
                return this.thirdpartyApiManager.ListNoticesDemoData(input.WorkShopCode);
            }

            var notices = await this.noticeRepository.GetAll()
                              .Where(n => n.RootDeviceGroupCode == input.WorkShopCode && n.IsActive).Select(n => n.Content)
                              .ToListAsync();

            return new ApiResponseObject(ApiItemType.Strings, ApiTargetType.None)
            {
                FixedSeries = true,
                Series = new List<IEnumerable<dynamic>>() { new List<string>() { "Content" } },
                Data = new List<IEnumerable<dynamic>>() { notices }
            };
        }

        public async Task<ApiResponseObject> ListToolWarnings(RequestDto input)
        {
            if (input.Demo)
            {
                return this.thirdpartyApiManager.ListToolWarningsDemoData(input.WorkShopCode);
            }

            return await this.cutterComponentManager.ListToolWarnings();

          
        }

        public async Task<ApiResponseObject> ListMachineStateDistribution(RequestDto input)
        {
            if (input.Demo)
            {
                return this.thirdpartyApiManager.ListMachineStateDistributionDemoData(input.WorkShopCode);
            }

            return await this.machineComponentManager.ListMachineStateDistribution(input.WorkShopCode);

           
        }

        public async Task<ApiResponseObject> ListMachineActivation(RequestDto input)
        {
            if (input.Demo)
            {
                return this.thirdpartyApiManager.ListMachineActivationDemoData(input.WorkShopCode);
            }

            var machineIdList = this.machineManager.ListMachinesInDeviceGroup(input.WorkShopCode).Select(s => s.Id).ToList();
            var currentMachineShiftDetailList = ( this.GetCurrentMachineShifDetailList(machineIdList)).Select(s => s.MachinesShiftDetailId).ToList();

            return await this.statisticalComponentManager.ListMachineActivation(input.WorkShopCode, currentMachineShiftDetailList);
            
        }

        public async Task<ApiResponseObject> ListMachineActivationByDay(RequestDto input)
        {
            if (input.Demo)
            {
                return this.thirdpartyApiManager.ListMachineActivationDemoData(input.WorkShopCode);
            }

            return await this.statisticalComponentManager.ListMachineActivationByDay(input.WorkShopCode);
        }

        public async Task<ApiResponseObject> ListCurrentShiftCapcity(RequestDto input)
        {
            if (input.Demo)
            {
                return this.thirdpartyApiManager.ListCurrentCapacityDemoData(input.WorkShopCode);
            }

            var machineIdList = await machineDeviceGroupRepository.GetAll().Where(dg => dg.DeviceGroupCode == input.WorkShopCode).Select(g => g.MachineId).ToListAsync();
            var machineShiftDetails =  GetCurrentMachineShifDetailList(machineIdList);

            return await this.statisticalComponentManager.ListCurrentShiftCapcity(machineShiftDetails);

        }

        public async Task<ApiResponseObject> ListGanttChart(RequestDto input)
        {
            if (input.Demo)
            {
                return this.thirdpartyApiManager.ListGanttChartDemoData(input.WorkShopCode);
            }

            return await this.machineComponentManager.ListGanttChart(input.WorkShopCode);
        }

        public async Task<ApiResponseObject> ListPlanRate(RequestDto input)
        {
            if (input.Demo)
            {
                return await this.thirdpartyApiManager.ListPlanRateDemoData(input.WorkShopCode);
            }

            return await this.planComponentManager.ListPlanRate();

        }


        public async Task<ApiResponseObject> ListConfigRealtimeMachineInfos(RequestDto input)
        {
            var machines = this.machineManager.ListMachinesInDeviceGroup(input.WorkShopCode).ToList();
            var documents = mongoMachineManager.ListOriginalMongoMachinesBsonDocument(machines.Select(m => m.Code)).ToList();

            if (input.Demo)
            {
                return this.thirdpartyApiManager.ListConfigRealtimeMachineInfosDemoData(documents, machines);
            }

            return await this.machineComponentManager.ListConfigRealtimeMachineInfos(documents, machines);
        }

        private List<CurrentMachineShiftInfoDto>GetCurrentMachineShifDetailList(List<int> machineIdList)
        {
            var mongoData = mongoMachineManager.OriginalMongoMachineList(machineIdList);

            if (mongoData==null)
            {
                return new List<CurrentMachineShiftInfoDto>();
            }

            return mongoData.Select(s => new CurrentMachineShiftInfoDto { MachineId =s.MachineId, MachinesShiftDetailId = s.MachinesShiftDetailId }).ToList();
        }

    }
}