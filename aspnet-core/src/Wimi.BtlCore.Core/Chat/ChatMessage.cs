using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Chat
{
    [Table("ChatMessages")]
    public class ChatMessage : Entity<long>, IHasCreationTime, IMayHaveTenant
    {
        public const int MaxMessageLength = 4 * 1024; //4KB

        [Comment("用户Id")]
        public long UserId { get; set; }

        [Comment("租户Id")]
        public int? TenantId { get; set; }

        [Comment("目标用户Id")]
        public long TargetUserId { get; set; }

        [Comment("目标租户Id")]
        public int? TargetTenantId { get; set; }

        [Required]
        [StringLength(MaxMessageLength)]
        [Comment("信息")]
        public string Message { get; set; }

        public DateTime CreationTime { get; set; }

        [Comment("发送方/接收方")]
        public ChatSide Side { get; set; }

        [Comment("是否已读")]
        public ChatMessageReadState ReadState { get; private set; }

        [Comment("接收方是否已读")]
        public ChatMessageReadState ReceiverReadState { get; private set; }

        [Comment("唯一标识(GUID)")]
        public Guid? SharedMessageId { get; set; }

        public ChatMessage(
            UserIdentifier user,
            UserIdentifier targetUser,
            ChatSide side,
            string message,
            ChatMessageReadState readState,
            Guid sharedMessageId,
            ChatMessageReadState receiverReadState)
        {
            UserId = user.UserId;
            TenantId = user.TenantId;
            TargetUserId = targetUser.UserId;
            TargetTenantId = targetUser.TenantId;
            Message = message;
            Side = side;
            ReadState = readState;
            SharedMessageId = sharedMessageId;
            ReceiverReadState = receiverReadState;

            CreationTime = Clock.Now;
        }

        public void ChangeReadState(ChatMessageReadState newState)
        {
            ReadState = newState;
        }

        protected ChatMessage()
        {

        }

        public void ChangeReceiverReadState(ChatMessageReadState newState)
        {
            ReceiverReadState = newState;
        }
    }
}
