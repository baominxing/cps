using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Wimi.BtlCore.Reasons.ReasonFeedbackCalendar.Dtos;
using WIMI.BTL.ReasonFeedbackCalendar.Dtos;

namespace WIMI.BTL.ReasonFeedbackCalendar
{
    public interface IFeedbackCalendarDetaillAppService:IApplicationService
    {
        Task<PagedResultDto<NameValueDto>> ListMachines(SelectMachinesInputDto input);

        Task<PagedResultDto<SelectMachineRelatedDto>> ListSelectedMachines(SelectMachinesInputDto input);

        Task AddMachinesToDetail(SelectMachineDto input);

        Task Delete(EntityDto<int> input);
    }
}