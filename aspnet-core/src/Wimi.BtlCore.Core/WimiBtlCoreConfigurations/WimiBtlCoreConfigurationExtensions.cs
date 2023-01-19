namespace Wimi.BtlCore.WimiBtlCoreConfigurations
{
    using Abp.Configuration.Startup;

    public static class WimiBtlCoreConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP MongoDb module.
        /// </summary>
        /// <param name="configurations">
        /// The configurations.
        /// </param>
        /// <returns>
        /// The <see cref="IWimiBtlCoreConfiguration"/>.
        /// </returns>
        public static IWimiBtlCoreConfiguration WimiBtlCore(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IWimiBtlCoreConfiguration>();
        }
    }
}