namespace Wimi.BtlCore.BasicData.Dto
{
    using Abp.Application.Services.Dto;

    public class AssignCustomerInputDto : NullableIdDto
    {
        public int? TenantId { get; set; }
    }
}