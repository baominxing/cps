using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Localization;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Machines;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.Order.DefectiveParts;
using Wimi.BtlCore.Order.DefectiveReasons;

namespace Wimi.BtlCore.Order.PartDefects.Manager
{
    public class PartDefectManager : BtlCoreDomainServiceBase, IPartDefectManager
    {
        private readonly IRepository<PartDefect> PartDefectRepository;

        private readonly IRepository<DefectivePart> DefectivePartRepository;

        private readonly IRepository<DefectiveReason> DefectiveReasonRepository;

        private readonly IRepository<DefectivePartReason> DefectivePartReasonRepository;

        private readonly IRepository<MachineDeviceGroup> MachineDeviceGroupRepository;

        private readonly IRepository<Machine> MachineRepository;

        private readonly IRepository<ShiftSolution> ShiftSolutionRepository;

        private readonly IRepository<ShiftSolutionItem> ShiftSolutionItemsRepository;

        private readonly ILocalizationManager localizationManager;

        public PartDefectManager(IRepository<DefectivePart> DefectivePartRepository,
            IRepository<DefectiveReason> DefectiveReasonRepository,
            IRepository<DefectivePartReason> DefectivePartReasonRepository,
            IRepository<MachineDeviceGroup> MachineDeviceGroupRepository,
            IRepository<Machine> MachineRepository,
            IRepository<ShiftSolution> ShiftSolutionRepository,
            IRepository<ShiftSolutionItem> ShiftSolutionItemsRepository,
            IRepository<PartDefect> PartDefectRepository,
            ILocalizationManager localizationManager)
        {
            this.DefectivePartRepository = DefectivePartRepository;
            this.DefectiveReasonRepository = DefectiveReasonRepository;
            this.DefectivePartReasonRepository = DefectivePartReasonRepository;
            this.MachineDeviceGroupRepository = MachineDeviceGroupRepository;
            this.MachineRepository = MachineRepository;
            this.ShiftSolutionRepository = ShiftSolutionRepository;
            this.ShiftSolutionItemsRepository = ShiftSolutionItemsRepository;
            this.PartDefectRepository = PartDefectRepository;
            this.localizationManager = localizationManager;
        }
        public IEnumerable<NameValueDto> ListDefectiveParts()
        {
            var query
                = this.DefectivePartRepository.GetAll().Select(d => new NameValueDto { Name = d.Name, Value = d.Id.ToString() }).Distinct();
            return query.ToList();
        }

        public IEnumerable<NameValueDto> ListDefectiveReasonsByPartId(EntityDto input)
        {
            var query
                = this.DefectivePartReasonRepository.GetAll()
                .Join(this.DefectiveReasonRepository.GetAll(), dpr => dpr.ReasonId, dr => dr.Id, (dpr, dr) => new { DefectivePartReason = dpr, DefectiveReason = dr })
                .Where(d => d.DefectivePartReason.PartId.Equals(input.Id))
                .Select(x => new NameValueDto { Name = x.DefectiveReason.Name, Value = x.DefectiveReason.Id.ToString() }).Distinct();
            return query.ToList();
        }

        public IEnumerable<NameValueDto> ListMachinesByDeviceGroupId(EntityDto input)
        {
            var query
                = this.MachineDeviceGroupRepository.GetAll()
                .Join(this.MachineRepository.GetAll(), mdg => mdg.MachineId, m => m.Id, (mdg, m) => new { MachineDeviceGroup = mdg, Machine = m })
                .WhereIf(!input.Id.Equals(0), d => d.MachineDeviceGroup.DeviceGroupId.Equals(input.Id))
                .OrderBy(m => m.Machine.SortSeq)
                .Select(x => new NameValueDto { Name = x.Machine.Name, Value = x.Machine.Id.ToString() }).Distinct();
            return query.ToList();
        }

        public IEnumerable<NameValueDto> ListShift()
        {
            var query
                = this.ShiftSolutionItemsRepository.GetAll()
                .Join(this.ShiftSolutionRepository.GetAll(), ssi => ssi.ShiftSolutionId, ss => ss.Id, (ssi, ss) => new { ShiftSolutionItem = ssi, ShiftSolution = ss })
                .Select(x => new NameValueDto { Name = x.ShiftSolution.Name + "-" + x.ShiftSolutionItem.Name, Value = x.ShiftSolutionItem.Id.ToString() }).Distinct().OrderBy(n => n.Value);
            return query.ToList();
        }

        public void CheckPartDefectiveInfo(PartDefect input)
        {
            var query
                = this.PartDefectRepository.FirstOrDefault(p => p.PartNo.Equals(input.PartNo)
                               && p.DefectivePartId.Equals(input.DefectivePartId)
                               && p.DefectiveReasonId.Equals(input.DefectiveReasonId)
                               && p.DefectiveMachineId.Equals(input.DefectiveMachineId));
            if (query != null)
            {
                var partName = this.DefectivePartRepository.Get(input.DefectivePartId).Name;
                var reasonName = this.DefectiveReasonRepository.Get(input.DefectiveReasonId).Name;
                var detectiveMachineName = this.MachineRepository.Get(input.DefectiveMachineId).Name;
                throw new UserFriendlyException(string.Format(localizationManager.GetString(BtlCoreConsts.LocalizationSourceName, "PartDefectHasSaved"), input.PartNo, detectiveMachineName, partName, reasonName));
            }
        }
    }
}
