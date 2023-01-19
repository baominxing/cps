using System;
using Wimi.BtlCore.BTLMongoDB.Repositories;
using Wimi.BtlCore.Extensions;
using Wimi.BtlCore.Parameters.Dto;

namespace Wimi.BtlCore.MongoException
{
    public class MongoExceptionManager: BtlCoreDomainServiceBase
    {
        public readonly MongoDbRepositoryBase<MongoException> mongoExceptionRepository;

        public MongoExceptionManager(MongoDbRepositoryBase<MongoException> mongoExceptionRepository)
        {
            this.mongoExceptionRepository = mongoExceptionRepository;
        }

        public void WriteExceptionToDb(string machineCode, string message, ErrorLevel errorLevel)
        {
            var exception = new MongoException()
            {
                MachineName = Environment.MachineName,
                ProcessName = "MDCDataSyncService",
                MachineCode = machineCode,
                Message = message,
                ErrorLevel = errorLevel,
                CreationTime = DateTime.Now.ToMongoDateTime()
            };
            mongoExceptionRepository.InsertOne(exception);
        }

    }
}
