using Abp;
using Abp.Domain.Repositories;
using Castle.Core.Internal;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Capacities;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Machines.Manager;
using Wimi.BtlCore.BasicData.Machines.Repository;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.ShiftDayTimeRange;
using Wimi.BtlCore.ThirdpartyApis.Interfaces;
using Wimi.BtlCore.ThirdpartyApis.Dto;
using Microsoft.EntityFrameworkCore;
using Wimi.BtlCore.BasicData.Machines.Repository.Dto;
using Wimi.BtlCore.Archives;
using Wimi.BtlCore.Machines.Mongo;
using Wimi.BtlCore.EfficiencyTrendas.Dtos;

namespace Wimi.BtlCore.ThirdpartyApis.Managers
{
    public class StatisticalComponentManager : AbpServiceBase, IStatisticalComponentManager
    {
        private readonly IRepository<Capacity> capacityRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly MachineManager machineManager;
        private readonly IShiftDetailRepository shiftDetailRepository; 
        private readonly IActivationRepository activationRepository;
        private readonly IRepository<ShiftSolutionItem> shiftSolutionItemsRepository;
        private readonly IShiftDayTimeRangeRepository shiftDayTimeRangeRepository;
        private readonly DeviceGroupManager deviceGroupManager;
        private readonly IRepository<ArchiveEntry> archiveEntryRepository;
        private readonly MongoMachineManager mongoMachineManager;
        public StatisticalComponentManager(IRepository<Capacity> capacityRepository,
                                    IRepository<Machine> machineRepository,
                                    IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
                                    MachineManager machineManager,
                                    IShiftDetailRepository shiftDetailRepository, 
                                    IActivationRepository activationRepository,
                                    IRepository<ShiftSolutionItem> shiftSolutionItemsRepository,
                                    IShiftDayTimeRangeRepository shiftDayTimeRangeRepository,
                                    DeviceGroupManager deviceGroupManager,
                                    IRepository<ArchiveEntry> archiveEntryRepository,
                                    MongoMachineManager mongoMachineManager)
        {
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.capacityRepository = capacityRepository;
            this.machineRepository = machineRepository;
            this.machineManager = machineManager;
            this.shiftDetailRepository = shiftDetailRepository; 
            this.activationRepository = activationRepository;
            this.shiftSolutionItemsRepository = shiftSolutionItemsRepository;
            this.shiftDayTimeRangeRepository = shiftDayTimeRangeRepository;
            this.deviceGroupManager = deviceGroupManager;
            this.archiveEntryRepository = archiveEntryRepository;
            this.mongoMachineManager = mongoMachineManager;
        }

        public async Task<ApiResponseObject> ListHourlyMachineYiled(string workShopCode)
        {
            var query = await this.ListHourlyYiled(workShopCode);
            var machines = this.machineManager.ListMachinesInDeviceGroup(workShopCode);

            var data = new List<IEnumerable<dynamic>>();
            var hour = new List<dynamic>();

            machines.ForEach(
                m =>
                {
                    var values = new List<dynamic>();
                    for (var i = 1; i <= 24; i++)
                    {
                        if (!hour.Contains(i)) hour.Add(i);
                        var hourDto = query.FirstOrDefault(n => n.Item1 == i);
                        values.Add(hourDto?.Item2.Where(t => t.MachineId == m.Id).Sum(n => n.Yield) ?? 0);
                    }

                    data.Add(values);
                });


            return new ApiResponseObject(ApiItemType.Numbers)
            {
                FixedSeries = true,
                Data = data,
                Target = machines.Select(n => n.Code).ToList(),
                Legend = machines.Select(n => n.Name).ToList(),
                Series = new List<IEnumerable<dynamic>>() { hour }
            };
        }

        public async Task<ApiResponseObject> ListPerHourYields(string workShopCode)
        {
            var query = await this.ListHourlyYiled(workShopCode);

            var data = new List<dynamic>();
            var hour = new List<dynamic>();
            for (var i = 1; i <= 24; i++)
            {
                var hourDto = query.FirstOrDefault(n => n.Item1 == i);
                data.Add(hourDto?.Item2.Sum(n => n.Yield) ?? 0);
                hour.Add(i);
            }

            return new ApiResponseObject(ApiItemType.Numbers, ApiTargetType.None)
            {
                FixedSeries = true,
                Data = new List<IEnumerable<dynamic>>() { data },
                Series = new List<IEnumerable<dynamic>>() { hour }
            };
        }

        public async Task<ApiResponseObject> ListHourlyMachineYiledByShiftDay(string workShopCode)
        {
            var query = await this.ListHourlyYiledByShiftDay(workShopCode);
            var machines = this.machineManager.ListMachinesInDeviceGroup(workShopCode);
            var machineIds = machines.Select(m => m.Id);

            var shiftDayTime = await this.shiftDayTimeRangeRepository.ListShiftDayTimeRanges(machineIds, DateTime.Today, DateTime.Today);
            var shiftDayTimeQuery = shiftDayTime.FirstOrDefault();
            if (shiftDayTimeQuery == null)
            {
                return new ApiResponseObject(ApiItemType.Numbers);
            }
            var shiftStartTime = shiftDayTimeQuery.StartTime;
            var shiftEndTime = shiftDayTimeQuery.EndTime;

            var data = new List<IEnumerable<dynamic>>();
            var hour = new List<dynamic>();
            machines.ForEach(
                m =>
                    {
                        var values = new List<dynamic>();
                        for (var i = shiftStartTime; i <= shiftEndTime; i = i.AddHours(1))
                        {
                            if (!hour.Contains(i.Hour)) hour.Add(i.Hour);
                            var hourDto = query.FirstOrDefault(n => n.Item1 == i.Hour);
                            values.Add(hourDto?.Item2.Where(t => t.MachineId == m.Id).Sum(n => n.Yield) ?? 0);
                        }

                        data.Add(values);
                    });


            return new ApiResponseObject(ApiItemType.Numbers)
            {
                FixedSeries = true,
                Data = data,
                Target = machines.Select(n => n.Code).ToList(),
                Legend = machines.Select(n => n.Name).ToList(),
                Series = new List<IEnumerable<dynamic>>() { hour }
            };
        }


        public async Task<ApiResponseObject> ListCurrentShiftCapcity(List<CurrentMachineShiftInfoDto> machineShiftDetails)
        {
            var shiftDetail = machineShiftDetails.Select(m => m.MachinesShiftDetailId).ToList();

            var query = await this.capacityRepository.GetAll().Where(c =>
                shiftDetail.Contains(c.MachinesShiftDetailId ?? 0) && c.MachinesShiftDetailId > 0 && c.IsLineOutput && c.Tag != "1").ToListAsync();

            var capacity = query.Any() ? query.Sum(n => n.Yield) : 0;

            var data = new List<dynamic>() { capacity };
            var series = new List<dynamic>() { "当前班次产量" };
            return new ApiResponseObject(ApiItemType.Numbers, ApiTargetType.None)
            {
                FixedSeries = true,
                Data = new List<IEnumerable<dynamic>>() { data },
                Series = new List<IEnumerable<dynamic>>() { series }
            };
        }

        public async Task<ApiResponseObject> ListMachineActivation(string workShopCode, List<int> currentMachineShiftDetailList)
        {
            var currentShiftMachineActivations =
                (await this.GetMachineActivationForDashboard(workShopCode, currentMachineShiftDetailList))
                .CurrentShiftMachineActivationsApi;

            var machineActivation = (from o in currentShiftMachineActivations
                                     join m in this.machineRepository.GetAllList() on o.MachineId equals m.Id
                                     orderby m.SortSeq descending
                                     select o).ToList();
            var machines = this.machineManager.ListMachinesInDeviceGroup(workShopCode).ToList();

            var data = new List<IEnumerable<dynamic>>();

            machines.ForEach(
                m =>
                {
                    var value = machineActivation.FirstOrDefault(q => q.MachineId == m.Id);
                    data.Add(new List<dynamic> { value?.Activation ?? 0 });
                });

            return new ApiResponseObject(ApiItemType.Numbers, ApiTargetType.None)
            {
                FixedSeries = true,
                Target = machines.Select(n => n.Code).ToList(),
                Legend = machines.Select(n => n.Name).ToList(),
                Data = data,
                Series = new List<IEnumerable<dynamic>>() { new[] { "当班次稼动率" } }
            };
        }

        public async Task<ApiResponseObject> ListMachineActivationByDay(string workShopCode)
        {
            var currentShiftMachineActivations =
                (await this.GetMachineActivationByDay(workShopCode))
                .CurrentShiftMachineActivationsApi;

            var machineActivation = (from o in currentShiftMachineActivations
                                     join m in this.machineRepository.GetAllList() on o.MachineId equals m.Id
                                     orderby m.SortSeq descending
                                     select o).ToList();
            var machines = this.machineManager.ListMachinesInDeviceGroup(workShopCode).ToList();

            var data = new List<IEnumerable<dynamic>>();

            machines.ForEach(
                m =>
                {
                    var value = machineActivation.FirstOrDefault(q => q.MachineId == m.Id);
                    data.Add(new List<dynamic> { value?.Activation ?? 0 });
                });

            return new ApiResponseObject(ApiItemType.Numbers, ApiTargetType.None)
            {
                FixedSeries = true,
                Target = machines.Select(n => n.Code).ToList(),
                Legend = machines.Select(n => n.Name).ToList(),
                Data = data,
                Series = new List<IEnumerable<dynamic>>() { new[] { "当天稼动率" } }
            };

        }

        private async Task<List<Tuple<int, IEnumerable<Capacity>>>> ListHourlyYiled(string workShopCode)
        {
            var query = (await (from c in this.capacityRepository.GetAll()
                               join m in this.machineRepository.GetAll() on c.MachineId equals m.Id
                               join mdg in this.machineDeviceGroupRepository.GetAll() on c.MachineId equals mdg.MachineId
                               where m.IsActive && c.StartTime >= DateTime.Today && c.StartTime <= DateTime.Now && mdg.DeviceGroupCode == workShopCode && c.Tag != "1"
                                select new { Capacity = c, mdg.DeviceGroupId }).ToListAsync())
                .GroupBy(q => q.Capacity.StartTime.Value.Hour, (key, g) => new { Hour = key, Yields = g })
                ;

            return query.Select(
                q => new Tuple<int, IEnumerable<Capacity>>(
                    q.Hour,
                    q.Yields.Select(
                        x => new Capacity()
                        {
                            Yield = x.Capacity.Yield,
                            MachineId = x.Capacity.MachineId,
                            StartTime = x.Capacity.StartTime,
                            MachineGroupInfo = new MachineGroupInfo() { GroupId = x.DeviceGroupId }

                        }))).ToList();
        }

        private async Task<List<Tuple<int, IEnumerable<Capacity>>>> ListHourlyYiledByShiftDay(string workShopCode)
        {
                var shifyDay = this.shiftDetailRepository.GetCurrentShiftDay().ShiftDay;

            if(shifyDay == new DateTime(1900, 01, 01))
            {
                var deviceGroups = await deviceGroupManager.ListDeviceGroupIdsForMachineShiftEffectiveInterval();
                if (deviceGroups.Any(x => x.Code.Equals(workShopCode)))
                {
                    shifyDay = DateTime.Today;
                }
            }
            var query =  (from c in capacityRepository.GetAll()
                               join m in machineRepository.GetAll() on c.MachineId equals m.Id
                               join mdg in machineDeviceGroupRepository.GetAll() on c.MachineId equals mdg.MachineId
                               where c.ShiftDetail.ShiftDay == shifyDay && m.IsActive && mdg.DeviceGroupCode == workShopCode && c.Tag != "1"
                               select new { Capacity = c, mdg.DeviceGroupId }).ToList()
                .GroupBy(q => q.Capacity.StartTime.Value.Hour, (key, g) => new { Hour = key, Yields = g })
                ;
            
            return query.Select(
                q => new Tuple<int, IEnumerable<Capacity>>(
                    q.Hour,
                    q.Yields.Select(
                        x => new Capacity()
                        {
                            Yield = x.Capacity.Yield,
                            MachineId = x.Capacity.MachineId,
                            StartTime = x.Capacity.StartTime,
                            MachineGroupInfo = new MachineGroupInfo() { GroupId = x.DeviceGroupId }

                        }))).ToList();
            
        }

        private async Task<MachineActivationApiDto> GetMachineActivationForDashboard(string workShopCode, List<int> currentMachineShiftDetailList)
        {
            var dto = new MachineActivationApiDto();

            var machineList = this.machineManager.ListMachinesInDeviceGroup(workShopCode).ToList();
            var machineIdList = machineList.Select(s => s.Id).ToList();
            var machineCalendars = this.shiftDetailRepository.GetShiftCalendarsByShiftIds(currentMachineShiftDetailList);

            try
            {
                // 获取当前班车设备稼动率

                dto.CurrentShiftMachineActivationsApi = (await this.GetMachineActivation(new EfficiencyTrendsInputDto()
                {
                    StatisticalWays = "ByMachineShiftDetail",
                    MachineId = machineIdList,
                    MachineShiftDetailId = currentMachineShiftDetailList,
                    QueryType = "0",
                    StartTime = machineCalendars.Min(s => s.StartTime),
                    EndTime = machineCalendars.Max(s => s.EndTime)
                }, machineList)).ToList();

            }
            catch (Exception e)
            {
                this.Logger.Fatal("首页-设备稼动率", e);
            }
            return dto;
        }
        private List<string> GetUnionTables(EfficiencyTrendsInputDto input)
        {
            var archiveTables = this.archiveEntryRepository.GetAll()
                .Where(s => s.TargetTable == "States").ToList()
                .Where(s => input.StartTime <= Convert.ToDateTime(s.ArchiveValue).Date && Convert.ToDateTime(s.ArchiveValue).Date <= input.EndTime)
                .GroupBy(s => s.ArchivedTable, s => s.ArchivedTable).Select(s => s.Key).ToList();

            return archiveTables;
        }
        private async Task<MachineActivationApiDto> GetMachineActivationByDay(string workShopCode)
        {
            var dto = new MachineActivationApiDto();

            var machineList = this.machineManager.ListMachinesInDeviceGroup(workShopCode).ToList();
            var machineIdList = machineList.Select(s => s.Id).ToList();
            var mongoMachine = this.mongoMachineManager.GetMongoMachineById(machineIdList.FirstOrDefault());
            try
            {
                // 获取当前班车设备稼动率
                var input = new EfficiencyTrendsInputDto()
                {
                    StatisticalWays = "ByDay",
                    MachineId = machineIdList,
                    QueryType = "0",
                    StartTime = Convert.ToDateTime(mongoMachine.ShiftExtras.ShiftDay),
                    EndTime = Convert.ToDateTime(mongoMachine.ShiftExtras.ShiftDay).AddDays(1)

                };
                dto.CurrentShiftMachineActivationsApi = (await this.GetMachineActivation(input, machineList)).ToList();

            }
            catch (Exception e)
            {
                this.Logger.Fatal("首页-设备稼动率", e);
            }
            return dto;
        }

        private async Task<IEnumerable<MachineActivationApiDto>> GetMachineActivation(EfficiencyTrendsInputDto input, List<Machine> machines)
        {
            var activation = await this.activationRepository.GetMachineActivationOriginalData( new EfficiencyTrendsInputDto()
                    {
                        StatisticalWays = input.StatisticalWays,
                        MachineId = input.MachineId,
                        MachineShiftDetailId = input.MachineShiftDetailId,
                        QueryType = "0",
                        StartTime = input.StartTime,
                        EndTime = input.EndTime,
                        UnionTables=this.GetUnionTables(input)
                    });

            if (activation == null)
            {
                throw new ArgumentNullException(nameof(activation));
            }

            var activationDictionary = new Dictionary<string, string>();

            const double tolerance = 0;
            foreach (var item in activation)
            {
                foreach (var subItem in item)
                {
                    if (subItem.Key == "Dimensions")
                    {
                        if (!activationDictionary.Keys.Contains(subItem.Key))
                        {
                            activationDictionary.Add(subItem.Key, subItem.Value.ToString());
                        }
                    }
                    else
                    {
                        var value = Convert.ToDouble(subItem.Value);

                        if (Math.Abs(value) <= tolerance)
                        {
                            continue;
                        }

                        if (!activationDictionary.Keys.Contains(subItem.Key))
                        {
                            activationDictionary.Add(subItem.Key, subItem.Value.ToString());
                        }
                        else
                        {
                            activationDictionary[subItem.Key] = value.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
            }

            var result = (from c in activationDictionary
                          join m in machines on c.Key equals m.Id.ToString()
                          select new MachineActivationApiDto
                          {
                              SortSeq = m.SortSeq,
                              MachineId = m.Id,
                              MachineName = m.Name,
                              Activation = Convert.ToDouble(c.Value)
                          });

            return result;
        }


    }
}