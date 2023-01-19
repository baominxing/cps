using Wimi.BtlCore.Order.Processes.Dtos;

namespace Wimi.BtlCore.Web.Models.Order.Process
{
    public class ProcessModel : ProcessDto
    {
        public ProcessModel()
        {
            this.IsEditMode = false;
        }

        public bool IsEditMode { get; set; }
    }
}
