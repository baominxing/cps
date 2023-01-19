using Abp.AutoMapper;
using System;
using Wimi.BtlCore.Cartons;

namespace Wimi.BtlCore.Carton.CartonTraceability.Dtos
{
    [AutoMap(typeof(CartonRecord))]
    public class CartonRecordDto
    {
        public int Id { get; set; }

        public string PartNo { get; set; }

        public bool Type { get; set; }

        public DateTime? OptionTime { get; set; }

        public DateTime ShiftDay { get; set; }

        public string ShiftSolutionItemName { get; set; }
    }
}
