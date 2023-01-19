using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Order.Products.Dtos;

namespace Wimi.BtlCore.Order.Product
{
    public interface IProductAppService : IApplicationService
    {
        Task CreateProduct(ProductRequestDto input);

        Task CreateProductGroup(ProductGroupRequestDto input);

        Task DeleteProduct(ProductRequestDto input);

        Task DeleteProductGroup(ProductGroupRequestDto input);

        Task<ProductDto> GetProductForEdit(ProductRequestDto input);

        Task<ProductGroupDto> GetProductGroupForEdit(ProductGroupRequestDto input);

        Task<IEnumerable<ProductGroupDto>> GetProductGroups();

        Task<IEnumerable<ProductDto>> GetProducts(ProductRequestDto input);

        Task UpdateProduct(ProductRequestDto input);

        Task UpdateProductGroup(ProductGroupRequestDto input);

        IEnumerable<ProductDto> ListProducts();

        Task<bool> ProductIsInProgress(ProductRequestDto input);

        Task<ListResultDto<GetProductsTreeDto>> GetProductsTree();
    }
}
