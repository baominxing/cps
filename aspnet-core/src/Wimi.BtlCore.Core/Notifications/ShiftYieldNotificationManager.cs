namespace Wimi.BtlCore.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Abp;
    using Abp.Collections.Extensions;
    using Abp.Domain.Repositories;
    using Abp.Localization;
    using Castle.Core.Internal;

    using Wimi.BtlCore.Authorization.Users;
    using Wimi.BtlCore.BasicData.Capacities;
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.BasicData.Shifts;

    public class ShiftYieldNotificationManager : WeixinNotificationBaseManager, IShiftYieldNotificationManager
    {
        private readonly IRepository<Capacity> capacityRepository;
        private readonly IRepository<MachinesShiftDetail> machineShiftDetailRepository;
        private readonly IRepository<ShiftSolutionItem> shiftSolutionItemRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly ILocalizationManager localizationManager;

        public ShiftYieldNotificationManager(
            IRepository<User, long> useRepository,
            IRepository<NotificationRule> notificationRuleRepository,
            IRepository<NotificationRuleDetail> notificationRuleDetailRepository,
            IRepository<Capacity> capacityRepository,
            IRepository<MachinesShiftDetail> machineShiftDetailRepository,
            IRepository<ShiftSolutionItem> shiftSolutionItemRepository,
            IRepository<Machine> machineRepository,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            ILocalizationManager localizationManager)
            : base(useRepository, notificationRuleRepository, notificationRuleDetailRepository)
        {
            this.capacityRepository = capacityRepository;
            this.machineShiftDetailRepository = machineShiftDetailRepository;
            this.shiftSolutionItemRepository = shiftSolutionItemRepository;
            this.machineRepository = machineRepository;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.localizationManager = localizationManager;
        }

        private int ShiftSolutionItemId { get; set; }

        public async Task<IEnumerable<Capacity>> ListShiftYields()
        {
            var query = (from c in this.capacityRepository.GetAll()
                         join m in this.machineRepository.GetAll() on c.MachineId equals m.Id
                         join d in this.machineDeviceGroupRepository.GetAll() on c.MachineId equals d.MachineId
                         join msd in this.machineShiftDetailRepository.GetAll() on
                         new { Id = (int)c.MachineId, ShiftDetaildId = c.MachinesShiftDetailId } equals
                         new { Id = msd.MachineId, ShiftDetaildId = (int?)msd.Id }
                         join ssi in this.shiftSolutionItemRepository.GetAll() on
                         new { msd.ShiftSolutionId, ItemId = msd.ShiftSolutionItemId, msd.ShiftDay } equals
                         new { ssi.ShiftSolutionId, ItemId = ssi.Id, ShiftDay = DateTime.Today }
                         where msd.ShiftDay == DateTime.Today && msd.ShiftSolutionItemId == this.ShiftSolutionItemId
                         group new { c.MachineId, ssi.Name, msd.ShiftDay, c.Yield } by
                         new { c.MachineId, MachineName = m.Name, ShiftItemId = ssi.Id,  GroupId = d.DeviceGroupId, ssi.Name, msd.ShiftDay }
                         into g
                         select new
                                    {
                                        g.Key.MachineId,
                                        g.Key.MachineName,
                                        MachineShiftName = g.Key.Name,
                                        g.Key.ShiftDay,
                                        g.Key.GroupId,
                                        g.Key.ShiftItemId,
                                        Yield = g.Sum(p => p.Yield)
                                    }).ToList();

            var result = query.Select(
                q => new Capacity()
                         {
                             MachineId = q.MachineId,
                             Yield = q.Yield,
                             MachineGroupInfo = new MachineGroupInfo() { GroupId = q.GroupId, MachineName = q.MachineName },
                             ShiftDetail =
                                 new ShiftDefailInfo()
                                     {
                                         ShiftDay = q.ShiftDay,
                                         MachineShiftName = q.MachineShiftName
                                     },
                             ShiftSolutionItemId = q.ShiftItemId
                         });

            return await Task.FromResult(result);
        }

        public void SetShiftSolutionItemId(int shiftSolutionItemId)
        {
            this.ShiftSolutionItemId = shiftSolutionItemId;
        }

        protected override string GetTemplate()
        {
            return localizationManager.GetString(BtlCoreConsts.LocalizationSourceName, "MachineYieldTemplate");
        }

        protected override WeixinMessageDataText GetContent(string template, object data)
        {
            var capacity = data as WeixinMessageDataQueue<Capacity>;
            return capacity == null
                       ? null
                       : new WeixinMessageDataText(
                           string.Format(
                               template,
                               capacity.Value.MachineGroupInfo.MachineName,
                               capacity.Value.ShiftDetail.ShiftDay?.ToString("yyyy-MM-dd") ?? DateTime.Today.ToString("yyyy-MM-dd"),
                               capacity.Value.ShiftDetail.MachineShiftName,
                               capacity.Value.Yield));
        }

        protected override async Task<IEnumerable<NameValue<WeixinMessageDataDto>>> ParsingMessage(string agentid)
        {
            var messageData = await this.ListShiftYields();
            var result = await this.ParsingMessageForwardingRules(messageData);

            return result.Select(
                r => new NameValue<WeixinMessageDataDto>()
                         {
                             Name = r.Key,
                             Value = new WeixinMessageDataDto(
                                 this.GetContent(this.GetTemplate(), r),
                                 r.ToUser,
                                 agentid)
                         });
        }

        private async Task<IEnumerable<WeixinMessageDataQueue<Capacity>>> ParsingMessageForwardingRules(IEnumerable<Capacity> input)
        {
            var ruleList = this.GetNotificationRule(EnumMessageType.YieldStatistics);
            var accountList = this.GetUserWeixinAccount();
            var stateList = new List<WeixinMessageDataQueue<Capacity>>();

            foreach(var p in input)
            {
                var rule = ruleList
                          .Where(
                              n => n.Name.Contains(p.MachineGroupInfo.GroupId.ToString())
                                   && p.ShiftSolutionItemId == n.Value.TriggerCondition)
                          .OrderByDescending(n => n.Value.TriggerCondition).FirstOrDefault();
                if (rule == null) continue;

                var toUser = accountList
                    .Where(m => rule.Value.NoticeUserIds.Contains(m.Name) && !m.Value.IsNullOrEmpty())
                    .Select(u => u.Value).JoinAsString("|");
                if (!toUser.IsNullOrEmpty())
                    stateList.Add(
                        new WeixinMessageDataQueue<Capacity>()
                        {
                            ToUser = toUser,
                            Value = p,
                            Key = $"{p.ShiftDetail.ShiftDay?.ToString("yyyy-MM-dd") ?? DateTime.Today.ToString("yyyy-MM-dd")}#{p.ShiftSolutionItemId}#{p.MachineId}"
                        });
            }

            return await Task.FromResult(stateList);
        }
    }
}