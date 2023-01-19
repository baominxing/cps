namespace Wimi.BtlCore.RealtimeIndicators.Parameters.Dto
{
    using Abp.Application.Services.Dto;

    public class ParamtersInputDto : EntityDto
    {
        public string MachineCode { get; set; }

        public int MachineId { get; set; }
    }
}