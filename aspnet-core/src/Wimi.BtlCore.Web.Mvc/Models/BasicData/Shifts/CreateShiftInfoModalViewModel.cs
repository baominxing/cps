using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wimi.BtlCore.Web.Models.BasicData.Shifts
{
    public class CreateShiftInfoModalViewModel
    {
        public CreateShiftInfoModalViewModel()
        {
            this.IsUsing = false;
        }

        public CreateShiftInfoModalDetailViewModel[] CreateShiftInfoModalDetailViewModel { get; set; }

        public int ShiftSolutionId { get; set; }

        public EnumOperationState State { get; set; }

        public bool IsUsing { get; set; }

        public bool CanDelete { get; set; }
    }
}
