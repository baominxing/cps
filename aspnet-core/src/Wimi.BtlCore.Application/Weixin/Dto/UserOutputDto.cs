namespace Wimi.BtlCore.Weixin.Dto
{
    using System;

    [Serializable]
    public class UserOutputDto
    {
        public UserOutputDto(string name, string userName, string wechatId, long userId)
        {
            this.Name = name;
            this.UserName = userName;
            this.WeChatId = wechatId;
            this.UserId = userId;
        }

        public string Name { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public string WeChatId { get; set; }
    }
}