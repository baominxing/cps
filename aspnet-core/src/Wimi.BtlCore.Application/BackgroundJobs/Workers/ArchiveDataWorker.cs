using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.Threading.Timers;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wimi.BtlCore.Archives;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.MongoException;

namespace Wimi.BtlCore.BackgroundJobs.Workers
{
    public class ArchiveDataWorker : WorkerBase
    {
        private readonly IArchiveEntryAppService archiveEntryAppService;

        public ArchiveDataWorker(
            AbpTimer timer,
            MongoExceptionManager mongoExceptionManager,
            IRepository<ArchiveEntry> archiveEntryRepository,
            IArchiveEntryAppService archiveEntryAppService
            )
            : base(timer, mongoExceptionManager)
        {
            if (!int.TryParse(AppSettings.MongodbDatabase.SyncMongoDataTimerPeriod, out var periodConfigValue))
            {
                periodConfigValue = DefaultTimerPeriod;
            }

            this.Timer.Period = OneMinute * AppSettings.General.ArchiveDataWorkerPeriod;

            this.archiveEntryAppService = archiveEntryAppService;
        }

        /// <summary>
        /// BackgroundWorker定时将处理过程压入Job队列，时间间隔从Web.Config中读取
        /// </summary>
        protected override void DoWork()
        {
            if (!this.CheckJobIsEffective(typeof(ArchiveDataWorker)))
            {
                return;
            }

            BackgroundJob.Enqueue(() => this.Execute());
        }

        /// <summary>
        /// 处理数据同步逻辑
        /// 配置Job失败后重试次数为0,避免同步数据出现重复数据
        /// </summary>
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public void Execute()
        {
            var targetTables = new List<string> { "Alarms", "Capacities", "States"};
            var archiveDateTime = DateTime.Now.AddDays(AppSettings.General.ArchivedTimePeriod);

            Parallel.ForEach(targetTables, targetTable =>
            {
                this.archiveEntryAppService.Archive(targetTable, archiveDateTime);
            });

            Task.Run(() => { this.archiveEntryAppService.ArchiveForTraceCatalogs(archiveDateTime); });
        }
    }

}
