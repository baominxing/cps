using Abp.Dependency;

namespace Wimi.BtlCore.BTLMongoDB.Configuration
{
    public interface IAbpMongoDbModuleConfiguration : ISingletonDependency
    {
        string ConnectionString { get; set; }

        string DatatabaseName { get; set; }

        void Initialize();
    }
}