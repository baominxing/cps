using Abp.Application.Services.Dto;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Castle.Core.Logging;
using Hangfire;
using System;
using System.Linq;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.StateInfos;
using Wimi.BtlCore.Extensions;
using Wimi.BtlCore.Feedback;
using Wimi.BtlCore.Feedback.Manager;

namespace WIMI.BTL.ReasonFeedbackCalendar
{
    public class RecurringJobService : ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly IRepository<FeedbackCalendar> feedbackCalendarRepository;
        private readonly IRepository<ReasonFeedbackRecord> recordRepository;
        private readonly IRepository<StateInfo> stateRepository;
        private readonly IRepository<FeedbackCalendarDetail> detailRepository;
        private readonly IReasonFeedbackManager feedbackManager;
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private static readonly object Locker = new object();

        public RecurringJobService(IRepository<FeedbackCalendar> feedbackCalendarRepository, IRepository<ReasonFeedbackRecord> recordRepository, IRepository<StateInfo> stateRepository, IRepository<FeedbackCalendarDetail> detailRepository, IReasonFeedbackManager feedbackManager, IUnitOfWorkManager unitOfWorkManager)
        {
            this.feedbackCalendarRepository = feedbackCalendarRepository;
            this.recordRepository = recordRepository;
            this.stateRepository = stateRepository;
            this.detailRepository = detailRepository;
            this.feedbackManager = feedbackManager;
            this.unitOfWorkManager = unitOfWorkManager;
            this.Logger = NullLogger.Instance;
        }

        [AutomaticRetry(Attempts = 0)]
        public void Feedback(EntityDto input)
        {
            lock (Locker)
            {
                try
                {
                    using (var unitOfWork = this.unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
                    {
                        var calendar = this.feedbackCalendarRepository.Get(input.Id);
                        var detaill = this.detailRepository.GetAllList(d => d.FeedbackCalendarId == input.Id).ToList();
                        var stateInfoList = this.stateRepository.GetAllList(t => t.Type == EnumMachineStateType.Reason);

                        foreach (var item in detaill)
                        {
                            var startTime = Convert.ToDateTime(DateTime.Now.ToLocalFormat());
                            var endTime = Convert.ToDateTime(DateTime.Now.AddMinutes(calendar.Duration).ToLocalFormat());

                            var state = stateInfoList.FirstOrDefault(t => t.Code == calendar.StateCode);
                            if (state == null) continue;

                            // 要做时间验证
                            var validate = feedbackManager.CheckStartTime(new ReasonFeedbackRecord
                            {
                                MachineId = item.MachineId,
                                StartTime = startTime
                            });
                            if (!validate.Result.Equals("Ok"))
                            {
                                this.Logger.Error($"任务[{calendar.Name}]下设备[{item.MachineId}]时间冲突跳过执行 结果[{validate.Result}]，[{validate.ErrorReason?.StartTime} ~ {validate.ErrorReason?.EndTime}]区间内已有反馈");
                                continue;
                            }
                            recordRepository.Insert(new ReasonFeedbackRecord()
                            {
                                MachineId = item.MachineId,
                                Duration = calendar.Duration,
                                StartTime = startTime,
                                EndTime = endTime,
                                StateCode = calendar.StateCode,
                                EndUserId = 2,
                                CreatorUserId = 2,
                                StateId = state.Id
                            });
                        }
                        unitOfWork.Complete();
                    }

                }
                catch (Exception e)
                {
                    this.Logger.Error($"任务Id[{input.Id}]执行时失败，原因{e}");
                }
            }
        }
    }
}