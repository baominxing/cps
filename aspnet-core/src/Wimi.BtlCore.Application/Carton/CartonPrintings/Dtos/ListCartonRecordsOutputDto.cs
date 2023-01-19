using Abp.Application.Services.Dto;
using System;

namespace Wimi.BtlCore.Carton.CartonPrintings.Dtos
{
    public class ListCartonRecordsOutputDto : EntityDto
    {
        public string PartNo { get; set; }

        public DateTime ShiftDay { get; set; }

        public string ShiftName { get; set; }

        public DateTime CartonTime { get; set; }
    }
}
