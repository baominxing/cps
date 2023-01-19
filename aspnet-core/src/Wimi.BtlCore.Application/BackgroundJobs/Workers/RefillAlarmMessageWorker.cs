using Abp.Threading.Timers;
using Hangfire;
using System.Collections.Generic;
using System.Linq;
using Wimi.BtlCore.BasicData.Alarms;
using Wimi.BtlCore.BasicData.Alarms.Mongo;
using Wimi.BtlCore.Common;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.MongoException;
using Wimi.BtlCore.Parameters.Mongo;

namespace Wimi.BtlCore.BackgroundJobs.Workers
{
    public class RefillAlarmMessageWorker : WorkerBase
    {
        private const string AlarmText = "STD::AlarmText";
        private const string AlarmCode = "STD::AlarmNo";
        private const string MachineId = "MachineId";
        private readonly MongoAlarmManager mongoAlarmManager;
        private readonly MongoParameterManager mongoParameterManager;
        private readonly ICommonRepository commonRepository;

        public RefillAlarmMessageWorker(AbpTimer timer, 
            MongoExceptionManager mongoExceptionManager,
            MongoAlarmManager mongoAlarmManager,
            MongoParameterManager mongoParameterManager,
            ICommonRepository commonRepository
            ) : base(timer, mongoExceptionManager)
        {
            this.Timer.Period = 5 * 60 * 1000; // 每5分钟运行一次
            this.mongoAlarmManager = mongoAlarmManager;
            this.mongoParameterManager = mongoParameterManager;
            this.commonRepository = commonRepository;
        }

        protected override void DoWork()
        {
            if (!AppSettings.BackgroudJobConfig.RefillAlarmFeatureEnabled.ToLower().Equals("true"))
                return;

            if (!this.CheckJobIsEffective(typeof(RefillAlarmMessageWorker))) return;
            BackgroundJob.Enqueue(() => this.Execute());
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public void Execute()
        {
            var alarmInfos = commonRepository.RefillAlarmMessage();

            UpdateMongoParamAlarmMessage(alarmInfos, AlarmCode, AlarmText);
            UpdateMongoAlarmMessage(alarmInfos);
        }

        private void UpdateMongoParamAlarmMessage(IEnumerable<AlarmInfo> alarmInfos, string alarmNoName, string alarmTextName)
        {
            mongoParameterManager.UpdateMongoAlarmMessage(alarmInfos, alarmNoName, alarmTextName);
        }

        private void UpdateMongoAlarmMessage(IEnumerable<AlarmInfo> alarmInfos)
        {
            var emptyMessage = mongoAlarmManager.ListMongoAlarmsWithEmptyMessage();

            foreach (var item in emptyMessage)
            {
                var alarm = alarmInfos.FirstOrDefault(t => t.MachineId == item.MachineId && t.Code.ToLower().Equals(item.Code.ToLower()));
                if (alarm == null) return;

                mongoAlarmManager.UpdateMongoAlarmMessage(alarm, item);

            }
        }


    }
}