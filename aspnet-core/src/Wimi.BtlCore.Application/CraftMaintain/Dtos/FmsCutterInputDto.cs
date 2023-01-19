using Abp.Runtime.Validation;
using System;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.FmsCutters;

namespace Wimi.BtlCore.FmsCutter.Dto
{
     public class FmsCutterInputDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public int Id { get; set; }

        public int? MachineId { get; set; }

        public string CutterNo { get; set; }

        public string CutterCase { get; set; }

        public string Type { get; set; }

        public decimal  FmsLength { get; set; }

        public decimal Diameter { get; set; }

        public string CompensateNo { get; set; }

        public decimal LengthCompensate { get; set; }

        public decimal DiameterCompensate { get; set; }

        public decimal OriginalLife { get; set; }

        public decimal CurrentLife { get; set; }

        public decimal WarningLife { get; set; }

        public EnumFmsUseType UseType { get; set; }

        public EnumFmsCutterCountType CountType { get; set; }

        public EnumFmsCutterState State { get; set; }

        public bool IsDeleted { get; set; }

        public long? DeleterUserId { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public long? LastModifierUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public long? CreatorUserId { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "CutterNo";
            }
        }
    }
}
