using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Gdpr
{
    public interface IUserCollectedDataProvider
    {
        Task<List<FileDto>> GetFiles(UserIdentifier user);
    }
}
