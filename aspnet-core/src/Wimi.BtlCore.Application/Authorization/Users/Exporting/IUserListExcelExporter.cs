using System.Collections.Generic;
using Wimi.BtlCore.Authorization.Users.Dto;
using Wimi.BtlCore.Dto;

namespace Wimi.BtlCore.Authorization.Users.Exporting
{
    public interface IUserListExcelExporter
    {
        FileDto ExportToFile(List<UserListDto> userListDtos);
    }
}
