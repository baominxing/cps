using System.Threading.Tasks;
using Wimi.BtlCore.ShiftDayTimeRange;
using Wimi.BtlCore.ThirdpartyApis.Interfaces;

namespace Wimi.BtlCore.ThirdpartyApis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abp;
    using Abp.AutoMapper;
    using Abp.Domain.Repositories;
    using Abp.ObjectMapping;
    using Castle.Core.Internal;
    using MongoDB.Bson;
    using NUglify.Helpers;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.BasicData.Machines.Manager;
    using Wimi.BtlCore.ThirdpartyApis;

    public class ThirdpartyApiManager : BtlCoreDomainServiceBase,IThirdpartyApiManager
    {
        private readonly IRepository<ThirdpartyApi, Guid> thirdpartyRepository;
        private readonly IRepository<DeviceGroup> deviceGroupRepository;
        private readonly IMachineManager machineManager;
        private readonly DeviceGroupManager deviceGroupManager;
        private readonly IShiftDayTimeRangeRepository shiftDayTimeRangeRepository;
        private readonly IRepository<MachineGatherParam, long> machineGatherParamRepository;

        public ThirdpartyApiManager(IRepository<ThirdpartyApi, Guid> thirdpartyRepository, 
            IRepository<DeviceGroup> deviceGroupRepository, IMachineManager machineManager, 
            DeviceGroupManager deviceGroupManager,
            IRepository<MachineGatherParam, long> machineGatherParamRepository,
            IShiftDayTimeRangeRepository shiftDayTimeRangeRepository)
        {
            this.thirdpartyRepository = thirdpartyRepository;
            this.deviceGroupRepository = deviceGroupRepository;
            this.machineManager = machineManager;
            this.deviceGroupManager = deviceGroupManager;
            this.shiftDayTimeRangeRepository = shiftDayTimeRangeRepository;
            this.machineGatherParamRepository = machineGatherParamRepository;
        }

        public void DeleteNotExistApis(IEnumerable<string> apiCodes)
        {
            this.thirdpartyRepository.Delete(t => !apiCodes.Contains(t.Code));
        }

        //todo
        public void Save(ThirdpartyApiDefinition api)
        {
            var apiCount = this.thirdpartyRepository.Count(t => t.Code.ToLower().Equals(api.Code.ToLower()));
            if (apiCount == 0)
            {
                var entity = new ThirdpartyApi()
                {
                    Code = api.Code,
                    Name = api.Name,
                    Url =api.Url,
                    Type =api.Type
                };

                thirdpartyRepository.Insert(entity);

                //this.thirdpartyRepository.Insert(ObjectMapper.Map<ThirdpartyApi>(api));
            }
        }

        public ApiResponseObject ListRealtimeMachineInfoDemoData(string workShopCode)
        {
            var machines = this.machineManager.ListMachinesInDeviceGroup(workShopCode).ToList();
            var result = new ApiResponseObject(ApiItemType.Objects)
            {
                Target = machines.Select(m => m.Code).ToList(),
                Legend = machines.Select(m => m.Name).ToList(),
                FixedSeries = false
            };

            machines.ForEach(
                m =>
                {
                    result.Series.Add(new List<NameValue>
                    {
                        new NameValue("状态", "STD::Status".ToLower()),
                        new NameValue("产量计数器", "STD::YieldCounter".ToLower()),
                        new NameValue("主轴转速", "STD::SpindleSpeed".ToLower()),
                        new NameValue("主轴倍率", "STD::SpindleOverride".ToLower()),
                        new NameValue("进给值", "STD::FeedSpeed".ToLower()),
                        new NameValue("进给倍率", "STD::FeedOverride".ToLower()),
                        new NameValue("程序名", "STD::Program".ToLower())
                    });
                    result.Data.Add(new List<dynamic> {3, 33, 0, 100, 5920, 100, "O2000.NC"});
                });

            return result;
        }

        public ApiResponseObject ListNoticesDemoData(string workShopCode)
        {
            return new ApiResponseObject(ApiItemType.Strings, ApiTargetType.None)
            {
                FixedSeries = true,
                Series = new List<IEnumerable<dynamic>>() { new[] { "Content" } },
                Data = new List<IEnumerable<dynamic>>() { new[] { "欢迎使用微茗(WIMI)可视化系统！" } }
            };
        }

        public ApiResponseObject ListMachineAlarmingDemoData(string workShopCode)
        {
            var machines = this.machineManager.ListMachinesInDeviceGroup(workShopCode);
            var result = new ApiResponseObject(ApiItemType.Strings)
            {
                FixedSeries = true,
                Target = machines.Select(m => m.Code).ToList(),
                Legend = machines.Select(m => m.Name).ToList(),
                Series = new List<IEnumerable<dynamic>>() { new[] { "报警号", "报警信息" } }
            };

            machines.ForEach(m => { result.Data.Add(new List<string>() { "72", "072 程序存储器中程序的数量满。" }); });
            return result;
        }

        public ApiResponseObject ListMachineStateDistributionDemoData(string workShopCode)
        {
            var machines = this.machineManager.ListMachinesInDeviceGroup(workShopCode);
            var result = new ApiResponseObject(ApiItemType.Percent)
            {
                FixedSeries = true,
                Target = machines.Select(m => m.Code).ToList(),
                Legend = machines.Select(m => m.Name).ToList(),
                Series = new List<IEnumerable<dynamic>>() { new[] { "Offline", "Free", "Run", "Stop", "Debug" } }
            };

            machines.ForEach(
                m =>
                    {
                        var list = this.ListRandomData(5, typeof(double));
                        result.Data.Add(list);
                    });


            return result;
        }

        public ApiResponseObject ListPerHourYieldDemoData(string workShopCode)
        {
            var result = new ApiResponseObject(ApiItemType.Numbers, ApiTargetType.None)
            {
                FixedSeries = true,
                Series = new List<IEnumerable<dynamic>>()
                                {
                                    new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" }
                                }
            };

            

            result.Data.Add(this.ListRandomData(24, typeof(int)));
            return result;
        }

        public ApiResponseObject ListHourlyMachineYiledDemoData(string workShopCode)
        {
            var machines = this.machineManager.ListMachinesInDeviceGroup(workShopCode);
            var result = new ApiResponseObject(ApiItemType.Numbers)
            {
                FixedSeries = true,
                Target = machines.Select(m => m.Code).ToList(),
                Legend = machines.Select(m => m.Name).ToList(),
                Series = new List<IEnumerable<dynamic>>()
                        {
                            new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" }
                        }
            };
            machines.ForEach(
                m =>
                    {
                        result.Data.Add(this.ListRandomData(24, typeof(int)));
                    });
            return result;
        }

        public ApiResponseObject ListHourlyMachineYiledByShiftDayDemoData(string workShopCode)
        {
            var machines = this.machineManager.ListMachinesInDeviceGroup(workShopCode);
            var machineIds = machines.Select(m => m.Id);

            var shiftDayTime = this.shiftDayTimeRangeRepository.ListMachineShiftDayTimeRange(machineIds);

            var shiftDayTimeQuery = shiftDayTime.FirstOrDefault();
            if (shiftDayTimeQuery == null)
            {
                return new ApiResponseObject(ApiItemType.Numbers);
            }
            var shiftStartTime = shiftDayTimeQuery.StartTime;
            var shiftEndTime = shiftDayTimeQuery.EndTime;


            var hour = new List<dynamic>();
            machines.ForEach(
                m =>
                {
                    for (var i = shiftStartTime; i <= shiftEndTime; i = i.AddHours(1))
                    {
                        if (!hour.Contains(i.Hour)) hour.Add(i.Hour);
                        
                    }
                });
            var result = new ApiResponseObject(ApiItemType.Numbers)
            {
                FixedSeries = true,
                Target = machines.Select(m => m.Code).ToList(),
                Legend = machines.Select(m => m.Name).ToList(),
                Series = new List<IEnumerable<dynamic>>() { hour } 
            };
            machines.ForEach(
                m =>
                {
                    result.Data.Add(this.ListRandomData(24, typeof(int)));
                });
            return result;
        }

        public ApiResponseObject ListToolWarningsDemoData(string workShopCode)
        {
            return new ApiResponseObject(ApiItemType.Strings, ApiTargetType.None)
            {
                FixedSeries = true,
                Series = new List<IEnumerable<dynamic>>()
                                        {
                                            new List<dynamic>() { "设备Id", "设备名称", "刀具编号", "刀具刀位", "初始寿命", "剩余寿命", "已使用", "预警值", "使用状态", "寿命状态", "寿命计数方式" }
                                        },
                Data = new List<IEnumerable<dynamic>>()
                                      {
                                          new List<dynamic>() { 1, "设备一", "TS-X23000001", 8, 100, 100, 0, 0, "已装刀", "正常", "时间" }
                                      }
            };
        }

        public ApiResponseObject ListMachineActivationDemoData(string workShopCode)
        {

            var machines = this.machineManager.ListMachinesInDeviceGroup(workShopCode).ToList();

            var series = new List<IEnumerable<dynamic>>();

            var xAxis = new List<dynamic>();

            xAxis.Add("稼动率");
            series.Add(xAxis);

            //machines.ForEach(
            //   m =>
            //   {
            //       series.Add(xAxis);
            //   });

            var result = new ApiResponseObject(ApiItemType.Numbers, ApiTargetType.One)
            {
                FixedSeries = true,
                Target = machines.Select(m => m.Code).ToList(),
                Legend = machines.Select(m => m.Name).ToList(),
                Series = series
            };

            var machineCount = machines.Count;

            machines.ForEach(
                m =>
                {
                    result.Data.Add(this.ListRandomData(1, typeof(int)));
                });

            return result;
        }

        public ApiResponseObject ListCurrentCapacityDemoData(string workShopCode)
        {
            return new ApiResponseObject(ApiItemType.Numbers, ApiTargetType.None)
            {
                FixedSeries = true,
                Series = new List<IEnumerable<dynamic>>()
                                        {
                                            new List<dynamic>() { "当前班次产量" }
                                        },
                Data = new List<IEnumerable<dynamic>>()
                                      {
                                          new List<dynamic>() {999}
                                      }
            };
        }
        public ApiResponseObject ListGanttChartDemoData(string workShopCode)
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd");
            //获取设备信息
            var machines = this.machineManager.ListMachinesInDeviceGroup(workShopCode);
            var codes = machines.Select(m => m.Code).ToList();
            var names = machines.Select(m => m.Name).ToList();
            var data = new List<IEnumerable<dynamic>>();
            for (int i = 0; i < codes.Count; i++)
            {
                data.Add(new List<dynamic>() { time + " 00:00:00", time + " 02:30:00", codes[i], "Run", "#4cae4c", "运行" });
                data.Add(new List<dynamic>() { time + " 02:30:00", time + " 04:30:00", codes[i], "Offline", "#c4c4c4", "离线" });
                data.Add(new List<dynamic>() { time + " 04:30:00", time + " 07:30:00", codes[i], "Free", "#f2a332", "空闲" });
                data.Add(new List<dynamic>() { time + " 07:30:00", time + " 12:00:00", codes[i], "Stop", "#d43a36", "停机" });
                data.Add(new List<dynamic>() { time + " 12:00:00", time + " 15:30:00", codes[i], "Run", "#4cae4c", "运行" });
                data.Add(new List<dynamic>() { time + " 15:30:00", time + " 17:30:00", codes[i], "Stop", "#d43a36", "停机" });
                data.Add(new List<dynamic>() { time + " 17:30:00", time + " 20:30:00", codes[i], "Free", "#f2a332", "空闲" });
                data.Add(new List<dynamic>() { time + " 20:30:00", time + " 22:30:00", codes[i], "Debug", "#1d89cf", "设定" });
            }
            return new ApiResponseObject(ApiItemType.Objects, ApiTargetType.One)
            {
                FixedSeries = true,
                Data = data,
                StartTime = time + " 00:00:00",
                EndTime = time + " 22:30:00",
                Legend = names,
                Target = codes
            };
        }

        public async Task<ApiResponseObject> ListPlanRateDemoData(string workShopCode)
        {
            var deviceGroups = (await deviceGroupManager.ListFirstClassDeviceGroups()).ToList();

            var data = new List<IEnumerable<dynamic>>();

            deviceGroups.ForEach(d =>
            {
                data.Add(new List<dynamic> { d.DisplayName, "Plan0003", "TS-0002", 1000, 200, $"{200 / 1000f}%", "按天", 20, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 18, $"{18 / 20f}%" });
            });

            return new ApiResponseObject(ApiItemType.Strings, ApiTargetType.One)
            {
                FixedSeries = true,
                Series = new List<IEnumerable<dynamic>>()
                {
                    new List<dynamic> { "设备组","计划名称", "产品名称", "计划产量", "完成产量", "总达成率", "目标维度", "目标量", "统计日期", "日期完成量","统计日期目标达成率" }
                },
                Data = data,
                Target = deviceGroups.Select(n => n.Code).ToList(),
                Legend = deviceGroups.Select(n => n.DisplayName).ToList()
            };
        }

        private IEnumerable<dynamic> ListRandomData(int length, Type type)
        {
            var list = new List<dynamic>();
            for (var i = 0; i < length; i++)
            {
                list.Add(type == typeof(double) ? RandomHelper.GetRandomOf(new[] { 0.2143, 0.3120, 0.4110, 0.6850, 0.8512 }) : RandomHelper.GetRandom(1, 100));
            }

            return list;
        }

        public ApiResponseObject ListConfigRealtimeMachineInfosDemoData(List<BsonDocument> documents, List<Machine> machines)
        {
            var result = new ApiResponseObject(ApiItemType.Objects)
            {
                Target = machines.Select(m => m.Code).ToList(),
                Legend = machines.Select(m => m.Name).ToList(),
                FixedSeries = false
            };

            machines.ForEach(
                m =>
                {
                    var machineGatherParams = this.machineGatherParamRepository.GetAll().ToList();

                    var parameters = documents.Select(d => new NameValue<BsonDocument>()
                    {
                        Name = d["MachineCode"].AsString,
                        Value = d
                    });

                    var mongoMachineParameter = parameters.FirstOrDefault(p => p.Name.Equals(m.Code));
                    if (mongoMachineParameter == null) return;

                    var series = machineGatherParams.Where(mg => mg.MachineId == m.Id && mg.IsShowForVisual && mg.Code.ToUpper() != "STD::AlarmText".ToUpper()
                    && mg.Code.ToUpper() != "STD::AlarmNo".ToUpper()).OrderBy(s => s.SortSeq).ToList();

                    var mongoMachine = mongoMachineParameter.Value;
                    var machineParamter = mongoMachine.Contains("Parameter") && mongoMachine["Parameter"].AsBsonDocument.Any() ? mongoMachine.AsBsonDocument["Parameter"].AsBsonDocument : null;

                   
                    var serie = new List<NameValue>();

                    series.ForEach(r =>
                    {
                        serie.Add(new NameValue(r.Name, r.Code.ToLower()));
                    });

                    result.Series.Add(serie);
                    result.Data.Add(this.ListRandomData(serie.Count(), typeof(int)));
                });

            return result;
        }
    }
}