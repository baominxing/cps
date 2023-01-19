using Abp.Runtime.Validation;
using System.Collections.Generic;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Tongs.Dto
{
    public class TongInputDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int Capacity { get; set; }

        public List<int> CapacityList { get; set; } = new List<int>();

        public string ProgramA { get; set; }

        public string ProgramB { get; set; }

        public string ProgramC { get; set; }

        public string ProgramD { get; set; }

        public string ProgramE { get; set; }

        public string ProgramF { get; set; }

        public string Note { get; set; }

        public string ProgramName { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "Id";
            }
        }
    }
}
