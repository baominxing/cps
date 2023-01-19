using System;
using System.Threading.Tasks;

using Abp.Domain.Repositories;
using Abp.Localization;
using Abp.UI;
using Wimi.BtlCore.Authorization.Users;

namespace Wimi.BtlCore.Order.MachineProcesses
{
    public class MachineProcessManager : BtlCoreDomainServiceBase, IMachineProcessManager
    {
        private readonly IRepository<MachineProcess> machineProcessRepository;

        private readonly UserManager userManager;
        private readonly ILocalizationManager localizationManager;

        public MachineProcessManager(IRepository<MachineProcess> machineProcessRepository, UserManager userManager,
            ILocalizationManager localizationManager)
        {
            this.machineProcessRepository = machineProcessRepository;
            this.userManager = userManager;
            this.localizationManager = localizationManager;
        }

        public void CheckExsist(int id)
        {
            var machineProcess = this.machineProcessRepository.FirstOrDefault(m => m.Id == id);
            if (machineProcess == null)
            {
                throw new UserFriendlyException(localizationManager.GetString(BtlCoreConsts.LocalizationSourceName, "DataHasBeenDeleted"));
            }
        }

        public async Task ChangeMachineProduct(MachineProcess input)
        {
            var machineData = this.machineProcessRepository.FirstOrDefault(mp => mp.MachineId == input.MachineId && mp.EndTime == null);
            if (machineData != null)
            {
                machineData.EndTime = DateTime.Now;
                machineData.ChangeProductUserId = Convert.ToInt32(this.userManager.AbpSession.UserId);
                await this.machineProcessRepository.UpdateAsync(machineData);
            }

            await this.machineProcessRepository.InsertAsync(input);
        }
    }
}