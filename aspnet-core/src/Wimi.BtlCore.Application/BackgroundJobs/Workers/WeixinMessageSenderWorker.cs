namespace Wimi.BtlCore.BackgroundJobs.Workers
{
    using Abp.Configuration;
    using Abp.Threading.Timers;
    using Hangfire;
    using System.Threading.Tasks;
    using Wimi.BtlCore.Configuration;
    using Wimi.BtlCore.MongoException;
    using Wimi.BtlCore.Weixin;
    using Wimi.BtlCore.Weixin.Dto;

    public class WeixinMessageSenderWorker : WorkerBase
    {
        private readonly IWeixinAppService weixinAppService;

        public WeixinMessageSenderWorker(AbpTimer timer, MongoExceptionManager mongoExceptionManager, IWeixinAppService weixinAppService, ISettingManager settingManager)
            : base(timer, mongoExceptionManager)
        {
            this.Timer.Period = DefaultTimerPeriod;
            this.weixinAppService = weixinAppService;
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task Execute()
        {
            var list = await this.weixinAppService.ListWaitingMessageDatas();
            var token = this.weixinAppService.GetToken();

            foreach (var item in list)
            {
                this.weixinAppService.Send(
                    new WeixinMessageInputDto()
                    {
                        AccessToken = token,
                        UrlFormat = AppSettings.WeixinYqConfig.SendAddress,
                        Data = item
                    });
            }
        }

        protected override void DoWork()
        {
            if (!this.CheckJobIsEffective(typeof(WeixinMessageSenderWorker))) return;
            BackgroundJob.Enqueue(() => this.Execute());
        }
    }
}