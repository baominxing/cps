using System;
using System.Collections.Generic;
using System.Text;

namespace Wimi.BtlCore.Carton.CartonTraceability.Dtos
{
    public class CartonExportDto
    {
        public string CartonNo { get; set; }

        public string DeviceGroupName { get; set; }

        public int DeviceGroupId { get; set; }

        public int MaxPackingCount { get; set; }

        public int RealPackingCount { get; set; }

        public int PrintLabelCount { get; set; }

        public DateTime CreationTime { get; set; }

        public List<CartonDetailDto> Details { get; set; }

    }

    public class CartonDetailDto
    {
        public string PartNo { get; set; }

        public DateTime OperationTime { get; set; }

        public DateTime ShiftDay { get; set; }

        public string ShiftSolutionItemName { get; set; }
    }
}
