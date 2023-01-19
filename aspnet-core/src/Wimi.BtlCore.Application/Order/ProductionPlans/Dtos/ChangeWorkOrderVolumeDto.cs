using System;
using System.Collections.Generic;
using System.Text;

namespace Wimi.BtlCore.Order.ProductionPlans.Dtos
{
    public class ChangeWorkOrderVolumeDto
    {
        public int AimVolume { get; set; }

        public int PutVolume { get; set; }

        public int WorkOrderId { get; set; }
    }
}
