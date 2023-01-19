using Abp;

namespace Wimi.BtlCore.Web.Models.Account
{
    public class SwitchToLinkedAccountModel
    {
        public int? TargetTenantId { get; set; }

        public string TargetUrl { get; set; }

        public long TargetUserId { get; set; }

        public UserIdentifier ToUserIdentifier()
        {
            return new UserIdentifier(this.TargetTenantId, this.TargetUserId);
        }
    }
}