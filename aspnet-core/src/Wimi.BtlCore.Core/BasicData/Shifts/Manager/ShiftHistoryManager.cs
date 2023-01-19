namespace Wimi.BtlCore.BasicData.Shifts.Manager
{
    using System.Threading.Tasks;

    using Abp.Domain.Repositories;

    public class ShiftHistoryManager: BtlCoreDomainServiceBase,IShiftHistoryManager
    {
        private readonly IRepository<ShiftHistory> shiftHistoryRepository;

        public ShiftHistoryManager(IRepository<ShiftHistory> shiftHistoryRepository)
        {
            this.shiftHistoryRepository = shiftHistoryRepository;
        }

        public async Task SaveChangeRecord(ShiftHistory input)
        {
            var isExist = await this.RecordIsExist(input);
            if (!isExist)
            {
                await this.shiftHistoryRepository.InsertAsync(input);
            }
        }


        private async Task<bool> RecordIsExist(ShiftHistory input)
        {
            var result = await this.shiftHistoryRepository.FirstOrDefaultAsync(
                             s => s.MachineId == input.MachineId && s.ShiftDay == input.ShiftDay
                                  && s.MachineShiftDetailId == input.MachineShiftDetailId);

            return result != null;
        }
    }
}