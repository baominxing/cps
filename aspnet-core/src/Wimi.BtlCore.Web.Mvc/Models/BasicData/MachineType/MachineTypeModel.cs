using Wimi.BtlCore.BasicData.Dto;

namespace Wimi.BtlCore.Web.Models.BasicData.MachineType
{
    public class MachineTypeModel : MachineTypeDto
    {
        public MachineTypeModel()
        {
            this.IsEditMode = false;
        }

        public bool IsEditMode { get; set; }
    }
}
