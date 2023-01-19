using System;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Storage
{
    public interface IBinaryObjectManager
    {
        Task DeleteAsync(Guid id);

        Task<BinaryObject> GetOrNullAsync(Guid id);
        
        Task SaveAsync(BinaryObject file);
    }
}