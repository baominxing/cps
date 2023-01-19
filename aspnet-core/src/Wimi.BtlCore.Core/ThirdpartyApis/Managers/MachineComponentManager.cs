using Abp;
using Abp.Domain.Repositories;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Machines.Manager;
using Wimi.BtlCore.BasicData.StateInfos;
using Wimi.BtlCore.BasicData.States;
using Wimi.BtlCore.ShiftDayTimeRange;
using Wimi.BtlCore.ThirdpartyApis.Interfaces;

namespace Wimi.BtlCore.ThirdpartyApis.Managers
{
    public class MachineComponentManager: IMachineComponentManager
    {
        private const string Alarm = "Alarm";
        private const string Parameter = "Parameter";
        private const string AlarmNokey = "STD::AlarmNo";
        private const string AlarmTextKey = "STD::AlarmText";
        private readonly IRepository<State,long> stateRepository;
        private readonly IRepository<StateInfo> stateInfoRepository;
        private readonly IMachineManager machineManager;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<MachineGatherParam,long> machineGatherParamRepository;
        private readonly IShiftDayTimeRangeRepository shiftDayTimeRangeRepository;

        public MachineComponentManager(IRepository<State, long> stateRepository, 
                                       IMachineManager machineManager,
                                       IRepository<StateInfo> stateInfoRepository,
                                       IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
                                       IRepository<Machine> machineRepository,
                                       IRepository<MachineGatherParam, long> machineGatherParamRepository,
                                       IShiftDayTimeRangeRepository shiftDayTimeRangeRepository)
        {
            this.machineManager = machineManager;
            this.stateRepository = stateRepository;
            this.stateInfoRepository = stateInfoRepository;
            this.machineRepository = machineRepository;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.machineGatherParamRepository = machineGatherParamRepository;
            this.shiftDayTimeRangeRepository = shiftDayTimeRangeRepository;
        }

        public async Task<ApiResponseObject> ListMachineStateDistribution(string workShopCode)
        {
            var machines = this.machineManager.ListMachinesInDeviceGroup(workShopCode);

            var shiftDay = this.shiftDayTimeRangeRepository.ListMachineShiftDayTimeRange().FirstOrDefault();
            var startTime = Convert.ToDateTime(shiftDay.BeginTime);
            var endTime = Convert.ToDateTime(shiftDay.FinishTime);

            var statesInfo = await this.stateInfoRepository.GetAll().Where(s => s.IsStatic).OrderBy(x=>x.Id).ToListAsync();
            var states = await this.stateRepository.GetAll().Where(s => startTime <= s.StartTime && s.StartTime <= endTime).ToListAsync();

            var result = new ApiResponseObject(ApiItemType.Percent)
            {
                FixedSeries = true,
                Series = new List<IEnumerable<dynamic>>() { statesInfo.Select(s => s.Code) }
            };


            machines.ForEach(
                m =>
                {
                    var data = new List<dynamic>();
                    statesInfo.ForEach(
                        s =>
                        {
                            var timeNow = DateTime.Now;
                            var totalStates = states.Where(st => st.MachineId == m.Id).Sum(st =>
                            {
                                if (st.EndTime == null)
                                {
                                    return Convert.ToDecimal((timeNow - Convert.ToDateTime(st.StartTime)).TotalSeconds);
                                }
                                return st.Duration;
                            });
                            var list = states.Where(st => st.MachineId == m.Id && st.Code.Equals(s.Code)).Sum(st =>
                            {
                                if (st.EndTime == null)
                                {
                                    return Convert.ToDecimal((timeNow - Convert.ToDateTime(st.StartTime)).TotalSeconds);
                                }
                                return st.Duration;
                            });
                            var distribution = totalStates != 0 ? Math.Round(list / totalStates, 4) : 0;
                            data.Add(distribution);
                        });

                    result.Target.Add(m.Code);
                    result.Legend.Add(m.Name);
                    result.Data.Add(data);
                });

            return result;
        }

        public async Task<ApiResponseObject> ListGanttChart(string workShopCode)
        {
            var query = from mgr in machineDeviceGroupRepository.GetAll()
                        join m in machineRepository.GetAll() on mgr.MachineId equals m.Id
                        join s in stateRepository.GetAll() on m.Id equals s.MachineId
                        join si in stateInfoRepository.GetAll() on s.Code equals si.Code
                        where mgr.DeviceGroupCode.StartsWith(workShopCode)
                        select new
                        {
                            StateStartTime = s.StartTime,
                            StateEndTime = s.EndTime,
                            StateCode = s.Code,
                            StateHexCode = si.Hexcode,
                            StateDisplayName = si.DisplayName,
                            MachineCode = m.Code,
                            MachineName = m.Name
                        };

            var startTime = DateTime.Now.Date;
            var endTime = DateTime.Now;
            var queryTimeList = await query.Where(q => q.StateStartTime.Value.Date == startTime && q.StateStartTime < endTime).ToListAsync();

            var data = new List<IEnumerable<dynamic>>();
            foreach (var item in queryTimeList)
            {
                var itemList = new List<dynamic>
                {
                    Convert.ToDateTime(item.StateStartTime).ToString("yyyy-MM-dd HH:mm:ss"),
                    item.StateEndTime == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : Convert.ToDateTime(item.StateEndTime).ToString("yyyy-MM-dd HH:mm:ss"),
                    item.MachineCode,
                    item.StateCode,
                    item.StateHexCode,
                    item.StateDisplayName
                };
                data.Add(itemList);
            }
            var time = DateTime.Now.ToString("yyyy-MM-dd");
            var machines = this.machineManager.ListMachinesInDeviceGroup(workShopCode);
            return new ApiResponseObject(ApiItemType.Objects, ApiTargetType.One)
            {
                FixedSeries = true,
                StartTime = time + " 00:00:00",
                EndTime = time + " 23:59:59",

                Data = data,
                Legend = machines.Select(m => m.Name).ToList(),
                Target = machines.Select(m => m.Code).ToList(),

            };
        }

        public ApiResponseObject ListMachineAlarming(List<BsonDocument> documents,List<Machine> machines)
        {
            var result = new ApiResponseObject(ApiItemType.Strings)
            {
                FixedSeries = true,
                Series = new List<IEnumerable<dynamic>>() { new List<string>() { "报警号", "报警信息" } }
            };

            if (!documents.Any()) return result;

            var alarms = documents.Select(
                d => d.Contains(Alarm) && d[Alarm].AsBsonArray.Any()
                         ? new NameValue<BsonArray>
                         {
                             Name = d["MachineCode"].AsString,
                             Value = d[Alarm].AsBsonArray
                         }
                         : null).ToList();

            machines.ForEach(
                m =>
                {
                    var alarm = alarms.FirstOrDefault(x => x != null && x.Name.Equals(m.Code));
                    if (alarm != null)
                    {
                        var elements = alarm.Value.FirstOrDefault();
                        var code = elements != null && !elements["Code"].IsBsonNull ? elements["Code"].AsString : string.Empty;
                        var message = elements != null && !elements["Message"].IsBsonNull ? elements["Message"].AsString : string.Empty;
                        result.Data.AddRange(new List<IEnumerable<dynamic>> { new[] { code, message } });
                    }
                    else
                    {
                        result.Data.AddRange(new List<IEnumerable<dynamic>> { new[] { string.Empty, string.Empty } });
                    }

                    result.Legend.Add(m.Name);
                    result.Target.Add(m.Code);
                });

            return result;
        }

        public async Task<ApiResponseObject> ListRealtimeMachineInfos(List<BsonDocument> documents,List<Machine>machines)
        {
            var result = new ApiResponseObject(ApiItemType.Objects);

            if (!documents.Any()) return result;

            var parameters = documents.Select(d => new NameValue<BsonDocument>()
            {
                Name = d["MachineCode"].AsString,
                Value = d
            });


            var machineGatherParams = await this.machineGatherParamRepository.GetAll().ToListAsync();

            machines.ForEach(m =>
            {
                var mongoMachineParameter = parameters.FirstOrDefault(p => p.Name.Equals(m.Code));

                if (mongoMachineParameter == null) return;

                var series = machineGatherParams.Where(mg => mg.MachineId == m.Id && mg.IsShowForVisual && mg.Code.ToUpper() != AlarmTextKey.ToUpper()
                && mg.Code.ToUpper() != AlarmNokey.ToUpper()).OrderBy(s => s.SortSeq).ToList();

                var mongoMachine = mongoMachineParameter.Value;
                var machineParamter = mongoMachine.Contains(Parameter) && mongoMachine[Parameter].AsBsonDocument.Any() ? mongoMachine.AsBsonDocument[Parameter].AsBsonDocument : null;

                var data = new List<dynamic>();
                var serie = new List<dynamic>();

                series.ForEach(r =>
                {
                    if (machineParamter == null)
                    {
                        data.Add("");
                    }
                    else
                    {
                        var element = machineParamter.FirstOrDefault(t => t.Name.ToLower().Equals(r.Code.ToLower()));
                        data.Add(element == default(BsonElement) ? "" : element.Value);

                        serie.Add(new NameValue(r.Name, r.Code.ToLower()));
                    }
                });


                result.Series.Add(serie);
                result.Target.Add(m.Code);
                result.Legend.Add(m.Name);
                result.Data.Add(data);
            });


            return result;
        }

        public async Task<ApiResponseObject> ListConfigRealtimeMachineInfos(List<BsonDocument> documents, List<Machine> machines)
        {
            var result = new ApiResponseObject(ApiItemType.Objects);

            if (!documents.Any()) return result;

            var parameters = documents.Select(d => new NameValue<BsonDocument>()
            {
                Name = d["MachineCode"].AsString,
                Value = d
            });


            var machineGatherParams = await this.machineGatherParamRepository.GetAll().ToListAsync();

            machines.ForEach(m =>
            {
                var mongoMachineParameter = parameters.FirstOrDefault(p => p.Name.Equals(m.Code));

                if (mongoMachineParameter == null) return;

                var series = machineGatherParams.Where(mg => mg.MachineId == m.Id && mg.IsShowForVisual && mg.Code.ToUpper() != AlarmTextKey.ToUpper()
                && mg.Code.ToUpper() != AlarmNokey.ToUpper()).OrderBy(s => s.SortSeq).ToList();

                var mongoMachine = mongoMachineParameter.Value;
                var machineParamter = mongoMachine.Contains(Parameter) && mongoMachine[Parameter].AsBsonDocument.Any() ? mongoMachine.AsBsonDocument[Parameter].AsBsonDocument : null;

                var data = new List<dynamic>();
                var serie = new List<dynamic>();

                series.ForEach(r =>
                {
                    if (machineParamter == null)
                    {
                        data.Add("");
                    }
                    else
                    {
                        var element = machineParamter.FirstOrDefault(t => t.Name.ToLower().Equals(r.Code.ToLower()));
                        data.Add(element == default(BsonElement) ? "" : element.Value);

                        serie.Add(new NameValue(r.Name, r.Code.ToLower()));
                    }
                });


                result.Series.Add(serie);
                result.Target.Add(m.Code);
                result.Legend.Add(m.Name);
                result.Data.Add(data);
            });


            return result;
        }
    }
}