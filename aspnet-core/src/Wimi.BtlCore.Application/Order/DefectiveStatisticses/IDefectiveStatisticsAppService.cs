using System.Collections.Generic;
using System.Threading.Tasks;

using Abp.Application.Services;
using Wimi.BtlCore.Order.DefectiveStatisticses.Dtos;
using Wimi.BtlCore.Order.Products.Dtos;

namespace Wimi.BtlCore.Order.DefectiveStatisticses
{
    public interface IDefectiveStatisticsAppService : IApplicationService
    {
        Task<IEnumerable<DefectiveStatisticsDto>> DefectiveStatisticsList(DefectiveStatisticRequestDto input);

        Task<IEnumerable<ProductDto>> FindProductList();
    }
}
