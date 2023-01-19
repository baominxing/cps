namespace Wimi.BtlCore.Authorization.Users.Dto
{
    using Abp.Application.Services.Dto;

    public interface IDatatablesPagedResultRequest : IPagedResultRequest
    {
        int Length { get; set; }

        int Start { get; set; }
    }
}