using Abp.Domain.Services;

namespace Wimi.BtlCore
{
    public abstract class BtlCoreDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */

        protected BtlCoreDomainServiceBase()
        {
            LocalizationSourceName = BtlCoreConsts.LocalizationSourceName;
        }
    }
}