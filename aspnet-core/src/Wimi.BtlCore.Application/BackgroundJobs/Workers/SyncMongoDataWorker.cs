using Abp.Configuration;
using Abp.Threading.Timers;
using Hangfire;
using System;
using System.Threading.Tasks;
using Wimi.BtlCore.BackgroundJobs.Process;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.Extensions;
using Wimi.BtlCore.MongoException;

namespace Wimi.BtlCore.BackgroundJobs.Workers
{
    public class SyncMongoDataWorker : WorkerBase
    {
        private static string _returnMessage = string.Empty;

        private readonly AlarmProcess alarmProcess;
        private readonly StateProcess stateProcess;
        private readonly CapacityProcess capacityProcess;
        private readonly ICommonRepository commonRepository;

        public SyncMongoDataWorker(AbpTimer timer,
            MongoExceptionManager mongoExceptionManager,
            AlarmProcess alarmProcess,
            StateProcess stateProcess,
            CapacityProcess capacityProcess,
            ISettingManager settingManager,
            ICommonRepository commonRepository)
            : base(timer, mongoExceptionManager)
        {
            this.alarmProcess = alarmProcess;
            this.stateProcess = stateProcess;
            this.capacityProcess = capacityProcess;
            this.SettingManager = settingManager;
            this.commonRepository = commonRepository;

            if (!int.TryParse(AppSettings.MongodbDatabase.SyncMongoDataTimerPeriod, out var periodConfigValue))
            {
                periodConfigValue = DefaultTimerPeriod;
            }

            var period = periodConfigValue * OneMinute;
            this.Timer.Period = period;
        }

        /// <summary>
        /// 处理数据同步逻辑
        /// 配置Job失败后重试次数为0,避免同步数据出现重复数据
        /// </summary>
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public void Execute()
        {
            try
            {
                this.Logger.Info($"[{DateTime.Now.ToLocalFormat()}]开始同步");
                var alarmResult = false;
                var stateResult = false;
                var capacityResult = false;
                var t1 = Task.Run(() => { alarmResult = alarmProcess.Process("Alarm", "Alarms"); });
                var t2 = Task.Run(() => { stateResult = stateProcess.Process("State", "States"); });
                var t3 = Task.Run(() => { capacityResult = capacityProcess.Process("Capacity", "Capacities"); });

                Task.WaitAll(t1, t2, t3);

                // 同步完成后，执行sql 更新结束时间
                this.Logger.Info($"[{DateTime.Now.ToLocalFormat()}]插入SQL完成，开始更新缺省字段");

                _returnMessage += commonRepository.CallSpToSyncEndTime(alarmResult, stateResult, capacityResult);

                if (string.IsNullOrEmpty(_returnMessage)) return;
                this.WriteExceptionToDb(string.Empty, _returnMessage, Parameters.Dto.ErrorLevel.Level3);
            }
            catch (Exception ex)
            {
                this.Logger.Fatal("SyncMongoDataWorker同步失败:原因", ex);
                this.WriteExceptionToDb(string.Empty, ex.Message, Parameters.Dto.ErrorLevel.Level3);
            }
        }

        public void SyncState()
        {
            if (!this.CheckJobIsEffective(typeof(SyncMongoDataWorker))) return;
            stateProcess.Process("State", "States");
            commonRepository.UpdateColumnsAfterSyncState();
        }

        /// <summary>
        /// BackgroundWorker定时将处理过程压入Job队列，时间间隔从Web.Config中读取
        /// </summary>
        protected override void DoWork()
        {
            if (!this.CheckJobIsEffective(typeof(SyncMongoDataWorker))) return;
            BackgroundJob.Enqueue(() => this.Execute());
        }

    }
}