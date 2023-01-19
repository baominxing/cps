using System.Collections.Generic;
using Wimi.BtlCore.AppSystem.Caching.Dto;

namespace Wimi.BtlCore.Web.Models.Maintain
{
    public class MaintenanceViewModel
    {
        public IReadOnlyList<CacheDto> Caches { get; set; }
    }
}
