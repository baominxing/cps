using Abp.Application.Services;
using System.Collections.Generic;
using Wimi.BtlCore.Reasons.ReasonFeedbackAnalysis.Dtos;

namespace Wimi.BtlCore.Reasons.ReasonFeedbackAnalysis
{
    public interface IReasonFeedbackAnalysisAppService : IApplicationService
    {
        ReasonFeedBackAnalysisDto GetReasonFeedBackResult(ReasonFeedBackAnalysisRequestDto input);

        List<ReasonFeedBackAnalysisDetailDto> GetDetail(ReasonFeedBackAnalysisDetailRequestDto input);
    }
}
