using MongoDB.Driver;
using System.Collections.Generic;
using Wimi.BtlCore.BTLMongoDB.Repositories;

namespace Wimi.BtlCore.States.Mongo
{
    public class MongoStateManager: BtlCoreDomainServiceBase
    {

        private readonly MongoDbRepositoryBase<MongoState> mongoStateRepository;

        public MongoStateManager(MongoDbRepositoryBase<MongoState> mongoStateRepository)
        {
            this.mongoStateRepository = mongoStateRepository;
        }

        public IEnumerable<MongoState> GetMongoStateWithNoSync(string lastSyncDateTime)
        {
            var filter = Builders<MongoState>.Filter.Gt(s=>s.CreationTime, lastSyncDateTime);
            var dataNeedSync = mongoStateRepository.GetMany(filter);
            return dataNeedSync;
        }

        public IEnumerable<MongoState> GetLastStateRecord(string machineCode ,int limitNum)
        {
            var filter = Builders<MongoState>.Filter.Where (s=>s.MachineCode== machineCode);
            var sort = Builders<MongoState>.Sort.Descending(s=>s.CreationTime);
            var result = mongoStateRepository.GetManyByLimit(filter, sort, 1);
            return result;
        }

        public void InsertMongoState(MongoState mongoState)
        {
            this.mongoStateRepository.InsertOne(mongoState);
        }
    }
}
