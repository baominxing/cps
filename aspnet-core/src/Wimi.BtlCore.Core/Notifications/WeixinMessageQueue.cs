namespace Wimi.BtlCore.Notifications
{
    public class WeixinMessageQueue
    {
       public WeixinMessageDataDto Message { get; set; }

        public bool Send { get; set; }

        public string Key { get; set; }

        public string Receiver { get; set; }

        public EnumMessageType Type { get; set; }
    }
}