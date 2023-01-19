using Abp.Domain.Services;

namespace Wimi.BtlCore.ThirdpartyApis.Interfaces
{
    public interface IThirdpartyApiStore: IDomainService
    {
        void Initialize();
    }
}