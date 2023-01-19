using Abp.Runtime.Validation;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Carton.CartonPrintings.Dtos
{
    public class ListCartonRecordsInputDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string CartonNo { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "CartonTime DESC";
            }
        }
    }
}
