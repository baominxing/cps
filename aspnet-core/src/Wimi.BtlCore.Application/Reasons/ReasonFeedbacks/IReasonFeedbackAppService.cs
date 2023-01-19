using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Wimi.BtlCore.Reasons.ReasonFeedbacks.Dtos;
using WIMI.BTL.ReasonFeedback.Dto;

namespace Wimi.BtlCore.Reasons.ReasonFeedbacks
{
    public interface IReasonFeedbackAppService : IApplicationService
    {
        /// <summary>
        /// 通过GroupId获取选中组下面的Machine的原因反馈的详细信息
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// </returns>
        Task<IEnumerable<ReasonFeedbackDto>> ListReasonFeedbackInfo(EntityDto input);

        /// <summary>
        /// 通过MachineId获取Machine的原因反馈历史
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<ReasonFeedbackDto>> ListReasonFeedbackHistoryInfo(GetFeedbackHistoryDto input);

        /// <summary>
        /// 获取原因反馈的类型
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<GetFeedbackTypeDto>> ListFeedbackType();

        /// <summary>
        /// 创建原因反馈
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task CreateReasonFeedback(ReasonFeedbackDto input);

        /// <summary>
        /// 通过MachineId获取到原因反馈（一台设备同一时间只能进行一种原因反馈）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ReasonFeedbackDto> GetReasonFeedbackRecord(EntityDto input);

        /// <summary>
        /// 结束原因反馈
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task FinishReasonFeedbackRecord(ReasonFeedbackDto input);

        /// <summary>
        /// 检查原因反馈的开始时间是否重合
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CheckTimeResultDto> CheckStartTime(ReasonFeedbackDto input);

        Task<CheckTimeResultDto> CheckEndTime(ReasonFeedbackDto input);
    }
}
