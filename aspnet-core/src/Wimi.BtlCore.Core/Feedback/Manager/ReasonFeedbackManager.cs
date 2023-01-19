namespace Wimi.BtlCore.Feedback.Manager
{
    using System;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Threading.Tasks;

    using Abp.Domain.Repositories;
    using Abp.Localization;
    using Abp.UI;
using WIMI.BTL.ReasonFeedback.Dto;

    public class ReasonFeedbackManager : BtlCoreDomainServiceBase, IReasonFeedbackManager
    {
        private readonly IRepository<ReasonFeedbackRecord> reasonFeedbackRecordRepository;
        private readonly ILocalizationManager localizationManager;

        public ReasonFeedbackManager(IRepository<ReasonFeedbackRecord> reasonFeedbackRecordRepository,ILocalizationManager localizationManager)
        {
            this.reasonFeedbackRecordRepository = reasonFeedbackRecordRepository;
            this.localizationManager = localizationManager;
        }

        /// <summary>
        /// 创建原因反馈时进行检查
        /// </summary>
        /// <param name="machineId"></param>
        public void CreateReasonFeedbackCheck(int machineId)
        {
            var result =
                this.reasonFeedbackRecordRepository.FirstOrDefault(
                    r => r.MachineId == machineId && r.EndTime == null);
            if (result != null)
            {
                throw new UserFriendlyException(localizationManager.GetString(BtlCoreConsts.LocalizationSourceName, "DeviceInFeedback"));
            }
        }

        /// <summary>
        /// 结束原因反馈时进行检查
        /// </summary>
        /// <param name="machineId">
        /// </param>
        public ReasonFeedbackRecord FinishReasonFeedbackCheck(int machineId)
        {
            var result =
                this.reasonFeedbackRecordRepository.FirstOrDefault(
                    r => r.MachineId == machineId && r.EndTime == null);
            if (result == null)
            {
                throw new UserFriendlyException(localizationManager.GetString(BtlCoreConsts.LocalizationSourceName, "FeedbackAlreadyEnd"));
            }

            return result;
        }

        public CheckTimeResultDto CheckStartTime(ReasonFeedbackRecord input)
        {
            DateTime? startTime = input.StartTime;
            var result = this.reasonFeedbackRecordRepository.FirstOrDefault(r => r.MachineId == input.MachineId && startTime >= r.StartTime && (startTime < r.EndTime || r.EndTime == null));
            if (result != null)
            {
                return new CheckTimeResultDto("Error")
                {
                    ErrorReason = new ErrorReasonDto
                    {
                        StartTime = result.StartTime,
                        EndTime = result.EndTime
                    }

                };
            }
            return new CheckTimeResultDto("Ok");
        }

        // 目前设置原因反馈的时间不能超钱7天
        // true:表示已经重合，false：没有重合
        public CheckTimeResultDto CheckEndTime(ReasonFeedbackRecord input)
        {
            DateTime? startTime = input.StartTime;
            DateTime? endTime = input.EndTime;
            var limitDate = startTime.Value.AddDays(-7);
            var query = this.reasonFeedbackRecordRepository.GetAll()
                 .Where(r => r.MachineId == input.MachineId && r.EndTime != null && r.StartTime >= limitDate)
                 .Where(
                     r => (endTime > r.StartTime && endTime <= r.EndTime)
 || (startTime <= r.StartTime && endTime > r.StartTime))
                .ToList()
                .Select(x => x.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + x.EndTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A");

            if (query.Count() > 0)
            {
                return new CheckTimeResultDto("True") { ErrprTimeRange = string.Join(",", query.ToArray()) };
            }
            else
            {
                return new CheckTimeResultDto("False");
            }

        }

        public async Task ForcedFinishByMachine(int machineId,long? userId)
        {
            var query = this.reasonFeedbackRecordRepository.GetAll().Where(r=>r.MachineId == machineId && r.EndTime ==null);
            if (!query.Any()) return;
            foreach (var item in query)
            {
                var nowDate = DateTime.Now;
                item.EndTime = nowDate;
                item.Duration =Convert.ToDecimal((nowDate - item.StartTime).TotalMinutes);
                item.EndUserId =Convert.ToInt32(userId);
                await this.reasonFeedbackRecordRepository.UpdateAsync(item);
            }
        }
    }
}
