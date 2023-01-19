using Abp.AutoMapper;
using Abp.ObjectMapping;
using System.Linq;
using Wimi.BtlCore.Authorization.Users.Dto;

namespace Wimi.BtlCore.Web.Models.Users
{
    [AutoMapFrom(typeof(GetUserForEditOutputDto))]
    public class CreateOrEditUserModalViewModel : GetUserForEditOutputDto
    {
        public CreateOrEditUserModalViewModel(GetUserForEditOutputDto output)
        {
            this.ProfilePictureId = output.ProfilePictureId;
            this.Roles = output.Roles;
            this.User = output.User;
        }

        public int AssignedRoleCount
        {
            get
            {
                return this.Roles.Count(r => r.IsAssigned);
            }
        }

        public bool CanChangeUserName
        {
            get
            {
                return this.User.UserName != Authorization.Users.User.AdminUserName;
            }
        }

        public bool IsEditMode
        {
            get
            {
                return this.User.Id.HasValue;
            }
        }
    }
}
