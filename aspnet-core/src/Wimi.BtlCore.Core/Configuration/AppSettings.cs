using Abp.Dependency;
using Microsoft.Extensions.Configuration;
using System;

namespace Wimi.BtlCore.Configuration
{
    /// <summary>
    ///     Defines string constants for setting names in the application.
    ///     See <see cref="AppSettingProvider" /> for setting definitions.
    /// </summary>
    public static class AppSettings
    {
        private static readonly IConfiguration Configuration = IocManager.Instance.Resolve<IConfiguration>();

        #region From appsetting.json
        public static string ProjectTag => Configuration.GetValue("AgileConfig:appId", "BTL");

        public static class Visual
        {
            // 综合分析甘特图开始时间
            public static string StartHourInGantChart => "StartHourInGantChart";//ConfigurationManager.AppSettings["StartHourInGantChart"];
        }

        public static int MachineTreeLength => Convert.ToInt32(Configuration.GetValue("App:MachineTreeLength", "10"));

        public static int DefaultSearchMachineCount => Convert.ToInt32(Configuration.GetValue("App:DefaultSearchMachineCount", "10"));

        public static class General
        {
            public const string WebSiteRootAddress = "App.General.WebSiteRootAddress";

            public static int ArchivedTimePeriod = Convert.ToInt32(Configuration.GetValue("App:ArchivedTimePeriod", "60")) * -1;

            public static int ArchiveDataWorkerPeriod = Convert.ToInt32(Configuration.GetValue("App:ArchiveDataWorkerPeriod", "30"));

        }

        public static class Page
        {
            public static string PageSizeOptions => Configuration.GetValue("App:PageSizeOptions", "10,20,50,100");
        }
        #endregion

        #region From AgileConfig
        public static class Database
        {
            /// <summary>
            /// SQL Server数据库连接字符串
            /// </summary>
            public static string ConnectionString => Configuration.GetValue("SqlConnectionString", string.Empty);
        }

        public static class MongodbDatabase
        {
            /// <summary>
            /// MongoDB数据库连接字符串
            /// </summary>
            public static string ConnectionString => Configuration.GetValue("MongoConnectionString", string.Empty);

            /// <summary>
            /// MongoDB数据库名称
            /// </summary>
            public static string DatabaseName => Configuration.GetValue("MongoDbName", string.Empty);

            /// <summary>
            /// MongoDB数据同步频率（单位：分钟）
            /// </summary>
            public static string SyncMongoDataTimerPeriod => Configuration.GetValue("SyncMongoDataTimerPeriod", "5");
        }

        public static string KafkaBootstrapServers => Configuration.GetValue("KafkaBootstrapServers", "192.168.137.200:9092");

        public static string MongoDataTopic => Configuration.GetValue("MongoDataTopic", "MongoData");

        public static string WebSiteRootAddress => Configuration.GetValue("WebSiteRootAddress", "http://192.168.137.1:4200");

        public static class Dmp
        {
            public static string DmpAddress => Configuration.GetValue("DmpSiteRootAddress", "http://192.168.137.1:8085");

            public static class ErrCodes
            {
                public static string NONE = "0000";

                public static string ERROR = "0001";
            }
        }

        public static class Influxdb
        {
            /// <summary>
            /// Influxdb地址
            /// </summary>
            public static string Address => Configuration.GetValue("InfluxDBIpAddress", "http://192.168.137.200:8086");
        }

        public static class BackgroudJobConfig
        {
            /// <summary>
            /// 是否回填报警内容
            /// </summary>
            public static string RefillAlarmFeatureEnabled => Configuration.GetValue("RefillAlarmFeatureEnabled", false.ToString());
        }

        public static class Shift
        {
            /// <summary>
            /// 统计班外数据	
            /// </summary>
            public static bool ShiftTimeOutside => Convert.ToBoolean(Configuration.GetValue("ShiftTimeOutside", false.ToString()));

            /// <summary>
            /// 班次最小单位
            /// </summary>
            public static string ShiftTimeDuration => Configuration.GetValue("ShiftTimeDuration", "30");
        }

        public static class TraceabilityConfig
        {
            /// <summary>
            /// 统计追溯产量
            /// </summary>
            public static bool OfflineYield => Convert.ToBoolean(Configuration.GetValue("TraceabilityOfflineYield", false.ToString()));
        }

        /// <summary>
        /// DMP多实例
        /// </summary>
        public static bool IsDmpMachine => Convert.ToBoolean(Configuration.GetValue("IsDmpMachine", false.ToString()));

        /// <summary>
        /// 产量关联计划	
        /// </summary>
        public static bool IsRelatePlan => Convert.ToBoolean(Configuration.GetValue("IsRelatePlan", false.ToString()));

        /// <summary>
        /// 钉钉机器人地址
        /// </summary>
        public static string DingtalkRobotWebhook => Configuration.GetValue("DingtalkRobotWebhook", string.Empty);

        public static class WeixinYqConfig
        {
            /// <summary>
            /// 启用微信功能
            /// </summary>
            public static bool WeixinFeatureEnabled => Convert.ToBoolean(Configuration.GetValue("WeixinFeatureEnabled", false.ToString()));

            /// <summary>
            /// WeixinCorpId
            /// </summary>
            public static string WeixinCorpId => Configuration.GetValue("WeixinCorpId", string.Empty);

            /// <summary>
            /// WeixinCorpSecret
            /// </summary>
            public static string WeixinCorpSecret => Configuration.GetValue("WeixinCorpSecret", string.Empty);

            /// <summary>
            /// WeixinAgentid
            /// </summary>
            public static string WeixinAgentid => Configuration.GetValue("WeixinAgentid", string.Empty);

            /// <summary>
            /// 企业号发送消息地址
            /// </summary>
            public static string SendAddress = @"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={0}";
        }


        #endregion

        #region From SettingManager
        public static class MachineParameter
        {
            public static string FixedDataItems => "FixedDataItems";

            public static string GaugeParameters => "GaugeParameters";
        }

        public static class FooterControl
        {
            //是否显示右下角版权信息
            public static string IsShowCopyRight => "IsShowCopyRight";//ConfigurationManager.AppSettings["IsShowCopyRight"];
        }

        public static class TenantManagement
        {
            public const string AllowSelfRegistration = "App.TenantManagement.AllowSelfRegistration";

            public const string DefaultEdition = "App.TenantManagement.DefaultEdition";

            public const string IsNewRegisteredTenantActiveByDefault = "App.TenantManagement.IsNewRegisteredTenantActiveByDefault";

            public const string UseCaptchaOnRegistration = "App.TenantManagement.UseCaptchaOnRegistration";

            public const string SubscriptionExpireNotifyDayCount = "App.TenantManagement.SubscriptionExpireNotifyDayCount";
        }

        public static class UserManagement
        {
            public const string AllowSelfRegistration = "App.UserManagement.AllowSelfRegistration";

            public const string IsNewRegisteredUserActiveByDefault = "App.UserManagement.IsNewRegisteredUserActiveByDefault";

            public const string UseCaptchaOnRegistration = "App.UserManagement.UseCaptchaOnRegistration";
        }

        public static class CutterManagement
        {
            public const string LifeMethod = "App.CutterManagement.LifeMethod";
        }
        #endregion
    }
}