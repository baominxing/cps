namespace Wimi.BtlCore.Notifications
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///  微信发送数据项： 由于接口规范，只能使用小写，排除StyleCop外
    /// </summary>
    [SuppressMessage("ReSharper", "StyleCop.SA1300")]
    public class WeixinMessageDataDto
    {
        public WeixinMessageDataDto(WeixinMessageDataText text, string toUser, string agentId, string msgtype = "text", int saft = 0)
        {
            this.text = text;
            this.touser = toUser;
            this.agentid = agentId;
            this.msgtype = msgtype;
            this.safe = saft;
        }

        public string touser { get; set; }

        public string msgtype { get; set; }

        public WeixinMessageDataText text { get; set; }

        public int safe { get; set; }

        public string agentid { get; set; }
    }

    [SuppressMessage("ReSharper", "StyleCop.SA1300")]
    public class WeixinMessageDataText
    {
        public WeixinMessageDataText(string content)
        {
            this.content = content;
        }

        public string content { get; set; }
    }

    public class WeixinMessageDataQueue<T>
    {
        public string Key { get; set; }

        public string ToUser { get; set; }

        public T Value { get; set; }
    }
}