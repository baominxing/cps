namespace Wimi.BtlCore.Cutter.Dto
{
    using Abp.AutoMapper;

    [AutoMap(typeof(LoadOrUnLoadCuttersDto))]
    public class LoadOrUnLoadCuttersOperationDto
    {
        public int CutterTVlaue { get; set; }

        public int MachineId { get; set; }

        public int OperationType { get; set; }

        public long? OperatorUserId { get; set; }
    }
}