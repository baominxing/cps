namespace Wimi.BtlCore.BasicData.Dto
{
    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;
    using Wimi.BtlCore.BasicData.Machines;

    [AutoMap(typeof(MachineGatherParam))]
    public class UpdateGatherParamsInputDto : EntityDto<long>
    {
        public EnumParamsDisplayStyle DisplayStyle { get; set; }

        public string Hexcode { get; set; }

        public bool IsShowForStatus { get; set; }

        public bool IsShowForVisual { get; set; }

        public bool IsShowForParam { get; set; }

        public long MachineId { get; set; }

        public double Max { get; set; }

        public double Min { get; set; }

        public string Name { get; set; }

        public int SortSeq { get; set; }

        public string Unit { get; set; }
    }
}