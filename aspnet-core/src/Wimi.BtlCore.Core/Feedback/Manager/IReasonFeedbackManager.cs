namespace Wimi.BtlCore.Feedback.Manager
{
    using System.Threading.Tasks;

    using Abp.Domain.Services;
    using WIMI.BTL.ReasonFeedback.Dto;

    public interface IReasonFeedbackManager : IDomainService
    {
        void CreateReasonFeedbackCheck(int machineId);

        ReasonFeedbackRecord FinishReasonFeedbackCheck(int machineId);

        CheckTimeResultDto CheckStartTime(ReasonFeedbackRecord input);

        CheckTimeResultDto CheckEndTime(ReasonFeedbackRecord input);

        Task ForcedFinishByMachine(int machineId,long? userId);
    }
}
