using Abp.Domain.Services;
using Abp.Runtime.Caching;
using Flurl.Http;
using Flurl.Http.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Wimi.BtlCore.WebServices
{
    public class WebServiceManager : DomainService
    {
        private readonly ICacheManager cacheManager;

        public WebServiceManager(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        /// <summary>
        /// 获取Soap请求模板xml
        /// </summary>
        /// <param name="updateWorkCenterStatus"></param>
        /// <returns></returns>
        public string GetSoapRequestXmlTemplate(string interfaceName)
        {
            var templateXml = cacheManager.GetCache<string, string>("SoapRequestXmlTemplates").Get(interfaceName, _ =>
            {
                //根据资源名称从Assembly中获取此资源的Stream：Wimi.BtlCore是SoapRequestXmlTemplates.xml文件所在dll命名空间，且需要将此文件作为嵌入资源
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Wimi.BtlCore.SoapRequestXmlTemplates.xml");

                var doc = new XmlDocument();

                doc.Load(stream);

                var cdata = doc.SelectNodes("/templates/template");

                var e = XElement.Parse(doc.OuterXml).Descendants("template").Where(s => s.FirstAttribute.Value == interfaceName).FirstOrDefault();

                return e?.Value ?? string.Empty;
            });

            if (string.IsNullOrEmpty(templateXml))
            {
                throw new Exception($"{interfaceName}接口没有维护模板");
            }

            return templateXml;
        }

        /// <summary>
        /// 利用Flurl发送请求
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public async Task<XDocument> PostStringAsync(string requestUrl, string sendMessage)
        {
            var response = await requestUrl
                .WithTimeout(5)
                .WithHeader("Content-Type", "text/xml; charset=utf-8")
                .PostStringAsync(sendMessage).ReceiveXDocument();

            return response;
        }

        #region 调用Demo,这方法应该放到ApplicationService层，放在这里为了方便演示
        //public async Task PostStringAsync()
        //{
        //    var sendMessage = string.Empty;

        //    //根据传入的接口名称，获取报文模板
        //    var templateXml = this.webServiceManager.GetSoapRequestXmlTemplate("checkProcessLot");

        //    //编译模板，填充参数内容
        //    sendMessage = new SoapXmlCompiler()
        //          .AddKey("site", "site")
        //          .AddKey("processLot", "processLot")
        //          .CompileString(templateXml);


        //    var response = await this.webServiceManager.PostStringAsync("http://laptop-prf64gh1:8082/CheckProcessLotWSService?wsdl", sendMessage);

        //}
        #endregion
    }
}
