using Abp.Collections.Extensions;
using Abp.UI;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Wimi.BtlCore.BasicData.Alarms;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BTLMongoDB.Repositories;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.Extensions;
using Wimi.BtlCore.Parameters.Dto;

namespace Wimi.BtlCore.Parameters.Mongo
{
    public class MongoParameterManager: BtlCoreDomainServiceBase
    {
        private const string DefultColor = "#204D74";
        private readonly MongoDbRepositoryBase<MongoParameter> mongoParameterRepository;

        public MongoParameterManager(MongoDbRepositoryBase<MongoParameter> mongoParameterRepository)
        {
            this.mongoParameterRepository = mongoParameterRepository;
        }

        public IEnumerable<BsonDocument> GetPartsProcessParamters(string partNo, int? machineId, DateTime? operationTimeBegin, DateTime? operationTimeEnd)
        {
            var result = new List<BsonDocument>();
            var filter = Builders<BsonDocument>.Filter;
            var sort = Builders<BsonDocument>.Sort.Descending("CreationTime");
            var machineFilter = filter.Eq("MachineId", machineId);
            FilterDefinition<BsonDocument> dofilter = null;

            var paramerKeyList = new List<string>();

            if (!string.IsNullOrEmpty(partNo))
            {
                dofilter = filter.Eq("PartCode", partNo);
            }

            if (operationTimeBegin.HasValue)
            {
                var begin = operationTimeBegin.Value.ToString("yyyyMMddHHmmssffff");

                string end = string.Empty;
                if (operationTimeEnd.HasValue)
                {
                    end = operationTimeEnd.Value.ToString("yyyyMMddHHmmssffff");
                }
                else
                {
                    end = operationTimeBegin.Value.AddMinutes(5).ToString("yyyyMMddHHmmssffff");
                }

                if (dofilter != null)
                {
                    dofilter = dofilter & (filter.Gte("CreationTime", begin) & filter.Lte("CreationTime", end));
                }
                else
                {
                    dofilter = filter.Gte("CreationTime", begin) & filter.Lte("CreationTime", end);
                }

                paramerKeyList = GetParamerNames(begin, end).OrderByDescending(s => s).ToList();
            }

            if (dofilter == null)
            {
                throw new UserFriendlyException(this.L("IncompleteAfferentParameters"));
            }

            dofilter &= machineFilter;

            var project = Builders<BsonDocument>.Projection.Exclude("_id")
                    .Exclude("MachineId")
                    .Exclude("PartCode");

            if (paramerKeyList.Any())
            {
                foreach (var key in paramerKeyList)
                {
                    var collection = mongoParameterRepository.GetCollectionByName(key);
                    var targetDocument = collection.Find(dofilter).Project(project).Sort(sort).ToList();
                    result.AddRange(targetDocument);
                }
            }
            else
            {
                var collection = this.mongoParameterRepository.BsonDocumentCollection;
                result = collection.Find(dofilter).Project(project).ToList();
            }

            return result;
        }

        public IEnumerable<GetHistoryParamtersDataTableDto> GetHistoryParamters(HistoryParameterListRequestDto input, IEnumerable<MachineGatherParam> paramList,out int totalCount)
        {
            totalCount = 0;
            var document = new List<BsonDocument>();  
            var filter = Builders<BsonDocument>.Filter;

            var startTime = input. StartTime.PadRight(18, '0');
            var endTime = input.EndTime.PadRight(18, '0');

            var doFilter = filter.And(filter.Eq("MachineId", input.MachineId),
                filter.Gte("CreationTime", startTime),
                filter.Lte("CreationTime", endTime)
            );

            var sort = Builders<BsonDocument>.Sort.Descending("CreationTime");

            var paramerKeyList = GetParamerNames(input.StartTime, input.EndTime).OrderByDescending(s => s).ToList();
             
            int skipCount = input.Start;
            int? limitCount = input.Length;
            foreach (var key in paramerKeyList)
            {
                var collection = mongoParameterRepository.GetCollectionByName(key);
                var count = (int)collection.CountDocuments(doFilter);
                totalCount += count; 

                var targetDocument = collection.Find(doFilter)
                    .Sort(sort)
                    .Skip(skipCount < 0 ? 0 : skipCount)
                    .Limit(limitCount)
                    .ToList();

                if (limitCount > 0 || limitCount == null)
                {
                    document.AddRange(targetDocument);
                }

                limitCount -= targetDocument.Count;
                skipCount -= count;
            }

            var rowDatas = new List<GetHistoryParamtersDataTableDto>();
            if (!input.MachineId.HasValue) return rowDatas;
             
            foreach (var item in document)
            {
                var columnsData = new GetHistoryParamtersDataTableDto { ObjectId = item.GetValue("_id").AsObjectId };
                foreach (var column in paramList)
                {
                    var target = item.FirstOrDefault(c => c.Name.ToLower().Equals(column.Code.ToLower()));
                    if (!string.IsNullOrEmpty(target.Name))
                    {
                        var value = column.Code.ToLower().Equals("creationtime")
                                        ? target.Value.ToString().FormartMongoDateTimeWithMillisecond()
                                        : $"{target.Value} {column.Unit}";
                        columnsData.ParamData.AddIfNotContains(new KeyValuePair<string, string>(target.Name.ToLower(), value));
                    }
                    else
                    {
                        columnsData.ParamData.AddIfNotContains(new KeyValuePair<string, string>(column.Code.ToLower(), "--"));
                    }
                }

                rowDatas.Add(columnsData);
            } 

            return rowDatas;
        }

        public IEnumerable<BsonDocument> GetHistoryParamtersForExport(HistoryParameterListRequestDto input, IEnumerable<MachineGatherParam> paramList)
        {
            var parameters = new List<BsonDocument>();
            var filter = Builders<BsonDocument>.Filter;
            var startTime = input.StartTime.PadRight(18, '0');
            var endTime = input.EndTime.PadRight(18, '0');
            var doFilter = filter.And(filter.Eq("MachineId", input.MachineId), filter.Gte("CreationTime", startTime), filter.Lte("CreationTime", endTime));
            var sort = Builders<BsonDocument>.Sort.Descending("CreationTime");
            var paramerKeyList = GetParamerNames(input.StartTime, input.EndTime).OrderByDescending(s => s).ToList();

            foreach (var key in paramerKeyList)
            {
                var collection = mongoParameterRepository.GetCollectionByName(key);
                var count = (int)collection.CountDocuments(doFilter);

                var targetDocument = collection.Find(doFilter)
                    .Sort(sort)
                    .ToList();

                parameters.AddRange(targetDocument);
            }

            return parameters;
        }

        public IEnumerable<GetParamtersListDto> GetLastNRecords(long machineId,List<MachineGatherParam> machineParam)
        {
            var collection = mongoParameterRepository.BsonDocumentCollection;

            SortDefinition<BsonDocument> sort = new BsonDocument("CreationTime", -1);
            var filter = Builders<BsonDocument>.Filter.Eq("MachineId", machineId);
            var projection = Builders<BsonDocument>.Projection.Exclude("_id").Exclude("MachineId");

            var result = collection.Find(filter).Project(projection).Sort(sort).Limit(10).ToList();
            result.Reverse();

            return (from item in result
                    let tempList = item.Elements.ToList()
                    let parameters = (from t in tempList
                                      let obj = machineParam.FirstOrDefault(s => s.Code == t.Name)
                                      where obj != null
                                      select new ParamsItem { Name = obj.Name, Value = t.Value.ToString(), Hexcode = DefultColor }).ToList()
                    let time = item["CreationTime"].ToString().MongoDateTimeParseExact()
                    select new GetParamtersListDto
                    {
                        LineChartParamtersList = parameters,
                        CreationTime = time.ToString("HH:mm:ss")
                    }).ToList(); ;
        }

        public void UpdateMongoAlarmMessage(IEnumerable<AlarmInfo> alarmInfos, string alarmNoName, string alarmTextName)
        {
            var collection = this.mongoParameterRepository.BsonDocumentCollection;
            if (collection == null)
            {
                return;
            }

            var filter = Builders<BsonDocument>.Filter.Eq(alarmTextName, string.Empty);
            // 对获取的数据 做限制，防止超大的数据 引起内存不可控
            // 累积数据，留到下次任务再执行
            var emptyMessages = collection.Find(filter).Limit(100).ToList();

            var list = new List<WriteModel<BsonDocument>>();
            emptyMessages.ForEach(
                m =>
                {
                    var alarmCode = m.GetElement(alarmNoName);
                    var machineId = m.GetElement("MachineId");
                    if (alarmCode != default(BsonElement))
                    {
                        var alarm = alarmInfos.FirstOrDefault(t => t.MachineId == machineId.Value.AsInt32 && t.Code.ToLower().Equals(alarmCode.Value.AsString.ToLower()));
                        if (alarm == null) return;

                        var alarmfilter = filter & Builders<BsonDocument>.Filter.Eq("MachineId", machineId.Value.AsInt32);
                        var update = Builders<BsonDocument>.Update.Set(alarmTextName, alarm.Message);
                        list.Add(new UpdateManyModel<BsonDocument>(alarmfilter, update));
                    }
                });

            if (list.Any())
            {
                collection.BulkWrite(list);
            }
        }

        public List<string> GetParamerNames(string startTime, string endTime)
        {
            string parameterKey = "Parameter";
            var months = new List<string>();
            var startDate = DateTime.Parse(startTime.MongoDateTimeParseExact().ToString("yyyy-MM"));
            var endDate = DateTime.Parse(endTime.MongoDateTimeParseExact().ToString("yyyy-MM"));
            for (var date = startDate; date <= endDate; date = date.AddMonths(1))
            {
                months.Add(parameterKey + date.ToString("yyyyMM"));
            }
            return months;
        }

        public  IEnumerable<ParamsItem> SetBlockChartParamtersList(BsonDocument mongoData,IEnumerable<MachineGatherParam> machineParam,string stateCode)
        {
            var returnValue = new List<ParamsItem>();

            var machineGatherParams = machineParam as MachineGatherParam[] ?? machineParam.ToArray();
            if (machineGatherParams.Any(n => n.Code.ToLower().Trim().Equals("std::yieldcounter"))
                || machineGatherParams.Any(n => n.Code.ToLower().Trim().Equals("std::program")))
            {
                var capacityDocument = mongoData.Contains("Capacity") && !mongoData["Capacity"].IsBsonNull ? mongoData["Capacity"].AsBsonDocument : null;
                if (capacityDocument != null && !capacityDocument.IsBsonNull)
                {
                    var targetCountParam =
                        machineGatherParams.FirstOrDefault(m => m.Code.ToLower().Trim().Equals("std::yieldcounter"));
                    var targetProParam =
                        machineGatherParams.FirstOrDefault(m => m.Code.ToLower().Trim().Equals("std::program"));

                    if (targetCountParam != null)
                    {
                        BsonElement originalCount;
                        if (!capacityDocument.TryGetElement("OriginalCount", out originalCount))
                            originalCount = new BsonElement("OriginalCount", 0);

                        returnValue.Add(
                            new ParamsItem
                            {
                                Name = targetCountParam.Name,
                                Value =
                                        stateCode != EnumMachineState.Offline.ToString()
                                            ? $"{originalCount.Value} {targetCountParam.Unit}"
                                            : "--",
                                Seq = targetCountParam.SortSeq,
                                Type = (int)targetCountParam.DisplayStyle,
                                Hexcode = string.IsNullOrEmpty(targetCountParam.Hexcode)
                                                  ? DefultColor
                                                  : targetCountParam.Hexcode
                            });
                    }

                    if (targetProParam != null)
                    {
                        BsonElement programName;
                        if (!mongoData.TryGetElement("ProgramName", out programName))
                            programName = new BsonElement("ProgramName", "--");

                        returnValue.Add(
                            new ParamsItem
                            {
                                Name = targetProParam.Name,
                                Value =
                                        stateCode != EnumMachineState.Offline.ToString()
                                            ? $"{programName.Value} {targetProParam.Unit}"
                                            : "--",
                                Seq = targetProParam.SortSeq,
                                Type = (int)targetProParam.DisplayStyle,
                                Hexcode = string.IsNullOrEmpty(targetProParam.Hexcode)
                                                  ? DefultColor
                                                  : targetProParam.Hexcode
                            });
                    }
                }
            }

            var parameterDocument = mongoData.Contains("Parameter") && !mongoData["Parameter"].IsBsonNull ? mongoData["Parameter"].AsBsonDocument : null;
            var fixDataItems = SettingManager.GetSettingValue(AppSettings.MachineParameter.FixedDataItems).Split(',');
            if (parameterDocument != null && !parameterDocument.IsBsonNull
                && stateCode != EnumMachineState.Offline.ToString())
            {
                // 整合数据,只取需要的值到前台
                // 填写Code对应的Name，过滤数据
                var mongoParamList = parameterDocument
                    .Select(t => new Tuple<string, string>(t.Name.Trim(), t.Value.ToString().Trim()))
                    .ToList();

                returnValue.AddRange(
                    from item in machineGatherParams
                    where !fixDataItems.Contains(item.Code)
                    where !returnValue.Select(x=>x.Name).Contains(item.Name)
                    let targetParam = mongoParamList.FirstOrDefault(p => p.Item1 == item.Code)
                    where targetParam != null && item.DisplayStyle == EnumParamsDisplayStyle.BlockChart
                    let temp = targetParam == null ? "--" : $"{targetParam.Item2} {item.Unit}"
                    select new ParamsItem
                    {
                        Name = item.Name,
                        Value = temp,
                        Seq = item.SortSeq,
                        Type = (int)item.DisplayStyle,
                        Hexcode = string.IsNullOrEmpty(item.Hexcode) ? DefultColor : item.Hexcode
                    });;
            }
            else
            {
                returnValue.AddRange(
                    from item in machineGatherParams
                    where !fixDataItems.Contains(item.Code)
                    where !returnValue.Select(x => x.Name).Contains(item.Name)
                    where item.DisplayStyle == EnumParamsDisplayStyle.BlockChart
                    select new ParamsItem
                    {
                        Name = item.Name,
                        Value = "--",
                        Seq = item.SortSeq,
                        Type = (int)item.DisplayStyle,
                        Hexcode = DefultColor
                    });
            }

            return returnValue.Distinct().ToList();
        }

        public IEnumerable<ParamsItem> SetGaugeParamtersList(BsonDocument mongoData,IEnumerable<MachineGatherParam> machineParam,string stateCode)
        {
            var returnValue = new List<ParamsItem>();

            var parameterDocument = mongoData.Contains("Parameter") && !mongoData["Parameter"].IsBsonNull ? mongoData["Parameter"].AsBsonDocument : null;

            var fixedDataItems = SettingManager.GetSettingValue(AppSettings.MachineParameter.FixedDataItems).Split(',');
            if (parameterDocument != null && !parameterDocument.IsBsonNull
                && stateCode != EnumMachineState.Offline.ToString())
            {
                // 整合数据,只取需要的值到前台
                // 填写Code对应的Name，过滤数据
                var mongoParamList = parameterDocument
                    .Select(t => new Tuple<string, string>(t.Name.Trim(), t.Value.ToString().Trim()))
                    .ToList();

                foreach (var item in machineParam)
                {
                    if (fixedDataItems.Contains(item.Code)) continue;

                    var targetParam = mongoParamList.FirstOrDefault(p => p.Item1 == item.Code);
                    if (targetParam != null && item.DisplayStyle == EnumParamsDisplayStyle.GaugePanel)
                    {
                        decimal result;

                        if (!string.IsNullOrEmpty(targetParam.Item2)
                            && !decimal.TryParse(targetParam.Item2, out result))
                            throw new UserFriendlyException(this.L("InvalidParameterValue{0}{1}", item.Name, targetParam.Item2));

                        var item2 = string.IsNullOrEmpty(targetParam.Item2) ? "0" : targetParam.Item2;
                        returnValue.Add(new ParamsItem { Name = item.Name, Value = item2, Seq = item.SortSeq });
                    }
                }
            }
            else
            {
                returnValue.AddRange(
                    from item in machineParam
                    where !fixedDataItems.Contains(item.Code)
                    where item.DisplayStyle == EnumParamsDisplayStyle.GaugePanel
                    select new ParamsItem { Name = item.Name, Value = "0", Seq = item.SortSeq });
            }

            return returnValue;
        }

        public IEnumerable<ParamsItem> SetLineChartParamtersList(BsonDocument mongoData,IEnumerable<MachineGatherParam> machineParam,string stateCode)
        {
            var returnValue = new List<ParamsItem>();

            var parameterDocument = mongoData.Contains("Parameter") && !mongoData["Parameter"].IsBsonNull ? mongoData["Parameter"].AsBsonDocument : null;
            var fixedDataItems = SettingManager.GetSettingValue(AppSettings.MachineParameter.FixedDataItems).Split(',');
            if (parameterDocument != null && !parameterDocument.IsBsonNull
                && stateCode != EnumMachineState.Offline.ToString())
            {
                // 整合数据,只取需要的值到前台
                // 填写Code对应的Name，过滤数据
                var mongoParamList = parameterDocument
                    .Select(t => new Tuple<string, string>(t.Name.Trim(), t.Value.ToString().Trim()))
                    .ToList();

                foreach (var item in machineParam)
                {
                    if (fixedDataItems.Contains(item.Code)) continue;

                    var targetParam = mongoParamList.FirstOrDefault(p => p.Item1 == item.Code);
                    if (targetParam != null && item.DisplayStyle == EnumParamsDisplayStyle.LineChart)
                    {

                        //if (!string.IsNullOrEmpty(targetParam.Item2)
                        //    && !decimal.TryParse(targetParam.Item2, out result))
                        //    throw new UserFriendlyException($@"参数{item.Name}值[{targetParam.Item2}]不是有效数值，不能使用线性图表！");

                        returnValue.Add(
                            new ParamsItem
                            {
                                Name = item.Name,
                                Value = targetParam.Item2,//string.IsNullOrEmpty(targetParam.Item2) ? "0" : targetParam.Item2,
                                Seq = item.SortSeq,
                                Min = item.Min,
                                Max = item.Max
                            });
                    }
                }
            }
            else
            {
                returnValue.AddRange(
                    from item in machineParam
                    where !fixedDataItems.Contains(item.Code)
                    where item.DisplayStyle == EnumParamsDisplayStyle.LineChart
                    select new ParamsItem
                    {
                        Name = item.Name,
                        Value = "0",
                        Seq = item.SortSeq,
                        Min = item.Min,
                        Max = item.Max
                    });
            }

            return returnValue;
        }
    }
}
