using Abp.Dependency;
using MongoDB.Driver;

namespace Wimi.BtlCore.BTLMongoDB
{
    public interface IMongoDatabaseProvider:ITransientDependency
    {
        /// <summary>
        /// Gets the <see cref="MongoDatabase"/>.
        /// </summary>
        IMongoDatabase Database { get; }
    }
}