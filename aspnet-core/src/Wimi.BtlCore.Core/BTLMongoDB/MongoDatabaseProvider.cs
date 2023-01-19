using MongoDB.Driver;
using Wimi.BtlCore.BTLMongoDB.Configuration;

namespace Wimi.BtlCore.BTLMongoDB
{
    public class MongoDatabaseProvider:IMongoDatabaseProvider
    {
        private readonly IAbpMongoDbModuleConfiguration mongoDbModuleConfiguration;

        public MongoDatabaseProvider(IAbpMongoDbModuleConfiguration mongoDbModuleConfiguration)
        {
            this.mongoDbModuleConfiguration = mongoDbModuleConfiguration;
        }

        public IMongoDatabase Database => CreateMongoDatabase();

        public IMongoDatabase CreateMongoDatabase()
        {
            var client = new MongoClient(mongoDbModuleConfiguration.ConnectionString);
            var mongoDatabase = client.GetDatabase(mongoDbModuleConfiguration.DatatabaseName);
            return mongoDatabase; 
        }
    }
}