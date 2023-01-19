using Senparc.CO2NET.Helpers.Serializers;
using Senparc.Weixin;

namespace Wimi.BtlCore.Weixin.Dto
{

    public class WeixinMessageInputDto
    {
        /// <summary>
        /// 通用接口的AccessToken，非OAuth的。如果不需要，可以为null
        /// </summary>
        public string AccessToken { get; set; }

        public bool CheckValidationResult { get; set; }

        /// <summary>
        /// 如果是Get方式，可以为null
        /// </summary>
        public object Data { get; set; }

        public JsonSetting JsonSetting { get; set; }

        public CommonJsonSendType SendType { get; set; }

        /// <summary>
        /// 代理请求超时时间（毫秒）
        /// </summary>
        public int TimeOut { get; set; }

        public string UrlFormat { get; set; }
    }
}