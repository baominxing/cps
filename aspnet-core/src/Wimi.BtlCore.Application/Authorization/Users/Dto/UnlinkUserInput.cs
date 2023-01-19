using Abp;

namespace Wimi.BtlCore.Authorization.Users.Dto
{
    public class UnlinkUserInputDto
    {
        public int? TenantId { get; set; }

        public long UserId { get; set; }

        public UserIdentifier ToUserIdentifier()
        {
            return new UserIdentifier(TenantId, UserId);
        }
    }
}