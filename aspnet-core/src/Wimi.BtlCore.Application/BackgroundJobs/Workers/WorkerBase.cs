using System.Linq;
using Abp.Dependency;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Hangfire;
using MongoDB.Driver;
using System;
using Wimi.BtlCore.MongoException;
using Wimi.BtlCore.Extensions;

namespace Wimi.BtlCore.BackgroundJobs.Workers
{
    public class WorkerBase : PeriodicBackgroundWorkerBase, ISingletonDependency
    {

        protected const int OneMinute = 60 * 1000;
        protected const int DefaultTimerPeriod = 5 * OneMinute;
        private readonly MongoExceptionManager mongoExceptionManager;

        public WorkerBase(AbpTimer timer, MongoExceptionManager mongoExceptionManager)
       : base(timer)
        {
            this.mongoExceptionManager = mongoExceptionManager;
        }

        protected bool CheckJobIsEffective(Type jobType)
        {
            var queuejob = JobStorage.Current.GetMonitoringApi().Queues()
                .Where(q => q.FirstJobs.Any(f => f.Value.Job.Type == jobType)).ToList();

            if (queuejob.Any())
            {
                var message = $"{jobType.Name} 任务取消: hangfire【Queues】中有正在排队的任务,数量:{queuejob.Count}";
                this.Logger.Error(message);
                return false;
            }

            var jobList = JobStorage.Current.GetMonitoringApi().ProcessingJobs(0, int.MaxValue)
                .Where(n => n.Value.Job.Type == jobType).ToList();

            if (!jobList.Any()) return true;

            var error = $"{jobType.Name} 任务取消: hangfire【ProcessingJobs】中有正在执行的任务,数量:{jobList.Count}";

            this.Logger.Error(error);
            return false;
        }

        protected void WriteExceptionToDb(string machineCode, string message, Parameters.Dto.ErrorLevel errorLevel)
        {
            mongoExceptionManager.WriteExceptionToDb(machineCode, message, errorLevel);
        }

        protected override void DoWork()
        {
        }
    }
}