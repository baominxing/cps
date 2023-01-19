using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Reasons.ReasonFeedbackCalendar.Dtos;

namespace WIMI.BTL.ReasonFeedbackCalendar
{
    public interface IReasonFeedbackCalendarAppService:IApplicationService
    {
        Task<DatatablesPagedResultOutput<FeedbackCalendarDto>> ListFeedbackCalendar(FeedbackCalendarInputDto input);

        Task Create(FeedbackCalendarInputDto input);

        Task Update(FeedbackCalendarInputDto input);

        Task Delete(EntityDto input);

        Task<FeedbackCalendarDto> Get(FeedbackCalendarInputDto input);

        Task<IEnumerable<NameValueDto>> ListFeedbackState();
    }
}