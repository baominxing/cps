using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization.Users.Dto;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Authorization.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task CreateOrUpdateUser(CreateOrUpdateUserInputDto input);

        Task DeleteUser(EntityDto<long> input);

        Task<GetUserForEditOutputDto> GetUserForEdit(NullableIdDto<long> input);

        Task<GetUserPermissionsForEditOutputDto> GetUserPermissionsForEdit(EntityDto<long> input);

        Task<DatatablesPagedResultOutput<UserListDto>> GetUsers(GetUsersInputDto input);

        Task<FileDto> GetUsersToExcel();

        Task ResetUserSpecificPermissions(EntityDto<long> input);

        Task UnlockUser(EntityDto<long> input);

        Task UpdateUserPermissions(UpdateUserPermissionsInputDto input);
    }
}
