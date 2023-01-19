namespace Wimi.BtlCore.Authorization.Roles
{
    using Abp.Application.Services;
    using Abp.Application.Services.Dto;
    using System.Threading.Tasks;
    using Wimi.BtlCore.Authorization.Roles.Dto;

    /// <summary>
    ///     Application service that is used by 'role management' page.
    /// </summary>
    public interface IRoleAppService : IApplicationService
    {
        Task CreateOrUpdateRole(CreateOrUpdateRoleInputDto input);

        Task DeleteRole(EntityDto input);

        Task<GetRoleForEditOutputDto> GetRoleForEdit(NullableIdDto input);

        Task<ListResultDto<RoleListDto>> GetRoles(GetRolesInputDto input);
    }
}