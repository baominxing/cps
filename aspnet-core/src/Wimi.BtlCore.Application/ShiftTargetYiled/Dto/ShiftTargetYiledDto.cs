namespace Wimi.BtlCore.ShiftTargetYiled.Dto
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    using Abp.AutoMapper;
    using Abp.Domain.Entities.Auditing;

    [AutoMap(typeof(ShiftTargetYileds))]
    public class ShiftTargetYiledDto : FullAuditedEntity<int?>
    {
        [NotMapped]
        public DateTime EndTime { get; set; }

        public string ProductCode { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSpec { get; set; }

        public DateTime ShiftDay { get; set; }

        public string ShiftName { get; set; }

        public int ShiftSolutionItemId { get; set; }

        [NotMapped]
        public DateTime StartTime { get; set; }

        public int TargetYiled { get; set; }
    }
}