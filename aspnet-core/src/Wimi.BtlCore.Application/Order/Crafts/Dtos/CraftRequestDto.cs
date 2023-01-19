using Abp.AutoMapper;
using Abp.Runtime.Validation;
using System.Collections.Generic;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Order.Crafts.Dtos
{
    [AutoMap(typeof(Craft))]
    public class CraftRequestDto : PagedAndSortedInputDto, IShouldNormalize
    {
        public CraftRequestDto()
        {
            this.ProcessIdList = new List<int>();
        }

        public string Code { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public List<int> ProcessIdList { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "ProcessOrder";
            }
        }
    }
}
