namespace Wimi.BtlCore.BasicData.Shifts.Manager
{
    using System.Threading.Tasks;

    using Abp.Domain.Services;

    public interface IShiftHistoryManager : IDomainService
    {
        Task SaveChangeRecord(ShiftHistory input);
    }
}