using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Wimi.BtlCore.Order.Crafts.Dtos
{
    [AutoMap(typeof(CraftProcess))]
    public class CraftProcessRequestDto : EntityDto<int>
    {
        public int CraftId { get; set; }

        public int CycleRate { get; set; }

        public int Increment { get; set; }

        public bool IsLastProcess { get; set; }

        public string ProcessCode { get; set; }

        public int ProcessId { get; set; }

        public string ProcessName { get; set; }

        public int ProcessOrder { get; set; }

        public decimal StandardCostTime { get; set; }
    }
}
