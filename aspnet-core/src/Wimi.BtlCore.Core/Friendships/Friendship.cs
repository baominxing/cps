using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp;
using Abp.Authorization.Users;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Friendships
{
    [Table("Friendships")]
    public class Friendship : Entity<long>, IHasCreationTime, IMayHaveTenant
    {
        [Comment("用户Id")]
        public long UserId { get; set; }

        [Comment("租户Id")]
        public int? TenantId { get; set; }

        [Comment("关联用户Id")]
        public long FriendUserId { get; set; }

        [Comment("关联租户Id")]
        public int? FriendTenantId { get; set; }

        [Required]
        [MaxLength(AbpUserBase.MaxUserNameLength)]
        [Comment("关联用户名")]
        public string FriendUserName { get; set; }

        [Comment("关联租户名")]
        public string FriendTenancyName { get; set; }

        [Comment("关联用户图片标识")]
        public Guid? FriendProfilePictureId { get; set; }

        [Comment("状态")]
        public FriendshipState State { get; set; }

        public DateTime CreationTime { get; set; }
        DateTime IHasCreationTime.CreationTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Friendship(UserIdentifier user, UserIdentifier probableFriend, string probableFriendTenancyName, string probableFriendUserName, Guid? probableFriendProfilePictureId, FriendshipState state)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (probableFriend == null)
            {
                throw new ArgumentNullException(nameof(probableFriend));
            }

            if (!Enum.IsDefined(typeof(FriendshipState), state))
            {
                throw new Exception("Invalid FriendshipState value: " + state);
            }

            UserId = user.UserId;
            TenantId = user.TenantId;
            FriendUserId = probableFriend.UserId;
            FriendTenantId = probableFriend.TenantId;
            FriendTenancyName = probableFriendTenancyName;
            FriendUserName = probableFriendUserName;
            State = state;
            FriendProfilePictureId = probableFriendProfilePictureId;

            CreationTime = Clock.Now;
        }

        protected Friendship()
        {

        }
    }
}
