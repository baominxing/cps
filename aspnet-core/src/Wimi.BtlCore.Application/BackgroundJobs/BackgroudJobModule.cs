using Abp.Modules;
using Abp.Threading.BackgroundWorkers;
using Wimi.BtlCore.BackgroundJobs.Workers;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.BackgroundJobs
{
    public class BackgroudJobModule : AbpModule
    {
        public override void PostInitialize()
        {
            if (!this.Configuration.BackgroundJobs.IsJobExecutionEnabled) return;

            var workerManager = this.IocManager.Resolve<IBackgroundWorkerManager>();

            workerManager.Add(this.IocManager.Resolve<SyncMongoDataWorker>());
            workerManager.Add(this.IocManager.Resolve<CheckMachineShiftTimelyWorker>());
            workerManager.Add(this.IocManager.Resolve<ArchiveDataWorker>());

            if (AppSettings.WeixinYqConfig.WeixinFeatureEnabled)
            {
                workerManager.Add(this.IocManager.Resolve<WeixinMessageSenderWorker>());
            }

            workerManager.Add(this.IocManager.Resolve<RefillAlarmMessageWorker>());
        }
    }
}