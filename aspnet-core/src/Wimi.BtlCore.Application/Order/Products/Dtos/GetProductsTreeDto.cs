using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wimi.BtlCore.Order.Products.Dtos
{
    public class GetProductsTreeDto
    {
        public string GroupName { get; set; }

        public List<GetProductChildrenTreeDto> Children { get; set; }
        = new List<GetProductChildrenTreeDto>();

    }

    public class GetProductChildrenTreeDto : EntityDto
    {
        public string Name { get; set; }
    }
}
