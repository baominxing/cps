using Abp.Application.Services.Dto;
using Abp.Dependency;
using System.Threading.Tasks;

namespace Wimi.BtlCore.BasicData.Calendars
{
    public interface ICalendarRepository : ITransientDependency
    {
        Task<Calendar> GetCalendarsByKey(EntityDto input);
    }
}
