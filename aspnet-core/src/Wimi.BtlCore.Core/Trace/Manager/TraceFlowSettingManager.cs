using Abp.Domain.Repositories;
using Abp.Localization;
using Abp.UI;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Wimi.BtlCore.Trace.Manager
{
    public class TraceFlowSettingManager : BtlCoreDomainServiceBase
    {
        private readonly IRepository<TraceFlowSetting> traceFlowSettingRepository;
        private readonly IRepository<TraceRelatedMachine> traceRelateMachineRepository;
        private readonly ILocalizationManager localizationManager;

        public TraceFlowSettingManager(IRepository<TraceFlowSetting> traceFlowSettingRepository, IRepository<TraceRelatedMachine> traceRelateMachineRepository,ILocalizationManager localizationManager)
        {
            this.traceFlowSettingRepository = traceFlowSettingRepository;
            this.traceRelateMachineRepository = traceRelateMachineRepository;
            this.localizationManager = localizationManager;
        }

        public async Task<TraceFlowSetting> GetTraceFlowSettingById(int id)
        {
            var entity = await traceFlowSettingRepository.GetAll().Include(x=>x.RelatedMachines).FirstOrDefaultAsync(x=>x.Id == id);
            if (entity == null)
            {
                throw new UserFriendlyException(localizationManager.GetString(BtlCoreConsts.LocalizationSourceName, "QuerySettingsDoNotExist"));
            }

            return entity;
        }

        public void AddMachineToFlowSetting(TraceFlowSetting traceFlowSetting, TraceRelatedMachine machine)
        {
            if (traceFlowSetting.RelatedMachines.Any(q => q.MachineCode == machine.MachineCode && q.WorkingStationCode == machine.WorkingStationCode))
            {
                throw new UserFriendlyException(localizationManager.GetString(BtlCoreConsts.LocalizationSourceName, "AlreadyExistSameDevice"));
            }

            traceFlowSetting.RelatedMachines.Add(machine);
        }

        public void UpdateMachineInFlowSetting(TraceFlowSetting traceFlowSetting, TraceRelatedMachine machine)
        {
            var targetMachine = traceFlowSetting.RelatedMachines.FirstOrDefault(q => q.MachineId == machine.MachineId && q.WorkingStationCode == machine.WorkingStationCode);
            targetMachine.WorkingStationDisplayName = machine.WorkingStationDisplayName;
        }

        public void RemoveMachineFromFlowSetting(TraceFlowSetting traceFlowSetting, TraceRelatedMachine machine)
        {
            var targetMachine = traceFlowSetting.RelatedMachines.FirstOrDefault(q => q.MachineId == machine.MachineId && q.WorkingStationCode == machine.WorkingStationCode);

            traceRelateMachineRepository.Delete(targetMachine);

            if (targetMachine != null)
            {
                traceRelateMachineRepository.Delete(targetMachine);
                // traceFlowSetting.RelatedMachines.Remove(targetMachine);
            }
        }

        public async Task<int> CreateFlowSetting(TraceFlowSetting traceFlowSetting)
        {
            ValidateFlowSettingAsync(traceFlowSetting);

            var newEntityKey = await traceFlowSettingRepository.InsertAndGetIdAsync(traceFlowSetting);

            return newEntityKey;
        }

        public async Task DeleteFlowSettingById(int id)
        {
            await traceRelateMachineRepository.DeleteAsync(q => q.TraceFlowSettingId == id);
            await traceFlowSettingRepository.DeleteAsync(q => q.Id == id);
        }

        public async Task DeleteByDeviceGroupId(int id)
        {
            var traceFlowSetting = traceFlowSettingRepository.GetAll().Where(t => t.DeviceGroupId == id).Select(t => t.Id).ToList();
            foreach (var tfs in traceFlowSetting)
            {
                await this.DeleteFlowSettingById(tfs);
            }
        }

        public async Task UpdateFlowSetting(TraceFlowSetting traceFlowSetting)
        {
            ValidateFlowSettingAsync(traceFlowSetting);

            await traceFlowSettingRepository.UpdateAsync(traceFlowSetting);
        }

        private void ValidateFlowSettingAsync(TraceFlowSetting traceFlowSetting)
        {
            //if (traceFlowSetting.Id != 0 && (traceFlowSetting.Id == traceFlowSetting.PreFlowId || traceFlowSetting.Id == traceFlowSetting.NextFlowId))
            //{
            //    throw new UserFriendlyException("上下道流程不能选择当前流程");
            //}

            if (traceFlowSettingRepository.GetAll().Any(q => q.Code == traceFlowSetting.Code && q.DeviceGroupId == traceFlowSetting.DeviceGroupId && q.Id != traceFlowSetting.Id))
            {
                throw new UserFriendlyException(localizationManager.GetString(BtlCoreConsts.LocalizationSourceName, "FlowCodeAlreadyExist"));
            }

            //if (!traceFlowSetting.PreFlowId.HasValue && !traceFlowSetting.NextFlowId.HasValue)
            //{
            //    throw new UserFriendlyException("请设定上、下道序，两道序不能同时为空");
            //}

            //if (traceFlowSetting.PreFlowId == traceFlowSetting.NextFlowId && (traceFlowSetting.PreFlowId.HasValue || traceFlowSetting.NextFlowId.HasValue))
            //{
            //    throw new UserFriendlyException("上、下道序不能相同");
            //}

            //if (!traceFlowSetting.RelatedMachines.Any())
            //{
            //    throw new UserFriendlyException("请至少增加一台设备至流程");
            //}
        }

        public async Task RemoveMachineFromFlowSettingById(int machineId)
        {
            await this.traceRelateMachineRepository.DeleteAsync(trm=>trm.MachineId == machineId);
        }
    }
}
