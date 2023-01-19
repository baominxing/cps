using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using Wimi.BtlCore.BTLMongoDB.Repositories;
using Wimi.BtlCore.Extensions;
using Wimi.BtlCore.Machines.Mongo;

namespace Wimi.BtlCore.BasicData.Alarms.Mongo
{
    public class MongoAlarmManager: BtlCoreDomainServiceBase
    {
        public readonly MongoDbRepositoryBase<MongoAlarm> mongoAlarmRepository;
        public readonly MongoDbRepositoryBase<MongoMachine> mongoMachineRepository;

        public MongoAlarmManager(MongoDbRepositoryBase<MongoAlarm> mongoAlarmRepository,
            MongoDbRepositoryBase<MongoMachine> mongoMachineRepository)
        {
            this. mongoAlarmRepository = mongoAlarmRepository;
            this.mongoMachineRepository = mongoMachineRepository;
        }

        public IEnumerable<MongoAlarm> ListMongoAlarms()
        {
            return this.mongoAlarmRepository.GetAll().ToList();
        }

        public IEnumerable<MongoAlarm> ListMongoAlarmsWithEmptyMessage()
        {
            var filter = Builders<MongoAlarm>.Filter.Where(s => s.Message == string.Empty);
            var sort = new SortDefinitionBuilder<MongoAlarm>().Ascending("CreationTime");
            var emptyMessages = mongoAlarmRepository.GetManyByLimit(filter, sort, 100);
            return emptyMessages;
        }


        public void UpdateMongoAlarmMessage(AlarmInfo alarmInfo,MongoAlarm mongoAlarm)
        {
            var filter = Builders<MongoAlarm>.Filter.Where(s=>s.Message== string.Empty);
            var alarmfilter = filter & Builders<MongoAlarm>.Filter.Where(s=>s.MachineId== mongoAlarm.MachineId);
            var update = Builders<MongoAlarm>.Update.Set(s=>s.Message, alarmInfo.Message);
            mongoAlarmRepository.UpdateOne(alarmfilter, update);
        }


        public IEnumerable<MongoAlarm> GetMachineHistoryAlarm(string machineCode,int pageNo,int pageSize)
        {
            var filter = Builders<MongoAlarm>.Filter.Where (s=>s.MachineCode== machineCode);
            var sort = Builders<MongoAlarm>.Sort.Descending(s=>s.CreationTime);

            var skipNum = (pageNo - 1) * pageSize;
            var limitNum = pageSize;

            var docList = mongoAlarmRepository.GetManyByPage(filter, sort, skipNum, limitNum);

            return docList;
        }

        public bool IsAlarming(string machineCode, DateTime creationTime, string code)
        {
            var filter = Builders<MongoMachine>.Filter.Where(s=>s.MachineCode==machineCode);
            var documentList = mongoMachineRepository.Get(filter);

            if (documentList!=null && documentList.Alarm.Count > 0)
            {
                var firstOrDefault = documentList.Alarm.FirstOrDefault(t=>t.Code == code);
                if (firstOrDefault == null) return false;
                var machineAlarmCreationTime = firstOrDefault.CreationTime.Substring(0,14);
                return creationTime >= machineAlarmCreationTime.DateTimeParseExact14();
            }
            return false;
        }


        public IEnumerable<MongoAlarm> GetMongoAlarmWithNoSync(string lastSyncDateTime)
        {
            var filter = Builders<MongoAlarm>.Filter.Gt(s=>s.CreationTime, lastSyncDateTime);
            var dataNeedSync = mongoAlarmRepository.GetMany(filter).ToList();
            return dataNeedSync;
        }

        public void InsertMongoAlarm(MongoAlarm mongoAlarm)
        {
            this.mongoAlarmRepository.InsertOne(mongoAlarm);
        }

    }
}