using Abp.Domain.Services;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Wimi.BtlCore.BTLMongoDB.Repositories;

namespace Wimi.BtlCore.BasicData.Capacities.Mongo
{
    public class MongoCapacityManager : DomainService
    {
        public readonly MongoDbRepositoryBase<MongoCapacity> mongoCapacityRepository;

        public MongoCapacityManager(MongoDbRepositoryBase<MongoCapacity> mongoCapacityRepository)
        {
            this.mongoCapacityRepository = mongoCapacityRepository;
        }

        public IEnumerable<MongoCapacity> GetMongoCapacityWithNoSync(string lastSyncDateTime)
        {
            var filter = Builders<MongoCapacity>.Filter.Gt(s => s.CreationTime, lastSyncDateTime);
            var dataNeedSync = mongoCapacityRepository.GetMany(filter).ToList();
            return dataNeedSync;
        }

        public IEnumerable<MongoCapacity> GetLastCapacityRecord(string machineCode, int limitNum)
        {
            var filter = Builders<MongoCapacity>.Filter.Where(s => s.MachineCode == machineCode);
            var sort = Builders<MongoCapacity>.Sort.Descending(s => s.CreationTime);
            var result = mongoCapacityRepository.GetManyByLimit(filter, sort, 1);
            return result;
        }

        /// <summary>
        /// 获取当天第一笔产量
        /// </summary>
        /// <returns></returns>
        public MongoCapacity GetFirstCapacityRecordByDay(string machineCode)
        {
            var currentDayFirstCreationTime = DateTime.Now.ToString("yyyyMMdd").PadRight(14, '0');
            var filter = Builders<MongoCapacity>.Filter;
            var countFileter = filter.Where(s => s.MachineCode == machineCode)
                               & filter.Lt(s => s.CreationTime, currentDayFirstCreationTime);
            var sort = Builders<MongoCapacity>.Sort.Descending(s => s.CreationTime);
            var result = mongoCapacityRepository.GetManyByLimit(countFileter, sort, 1).FirstOrDefault();
            return result;
        }


        public void InsertMongoCapacity(MongoCapacity mongoCapacity)
        {
            this.mongoCapacityRepository.InsertOne(mongoCapacity);
        }
    }
}
