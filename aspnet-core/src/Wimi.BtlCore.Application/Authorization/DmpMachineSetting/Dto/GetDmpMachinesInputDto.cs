using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Runtime.Validation;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Authorization.DmpMachineSetting.Dto
{
    public class GetDmpMachinesInputDto : PagedAndSortedInputDto, IShouldNormalize
    {
        public int Id { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "SortSeq";
            }
        }
    }
}
