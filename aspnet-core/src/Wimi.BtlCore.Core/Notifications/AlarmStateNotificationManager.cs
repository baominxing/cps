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
    using Wimi.BtlCore.BasicData.DeviceGroups;
    using Wimi.BtlCore.BasicData.Machines;
    using Wimi.BtlCore.BasicData.States;

    public class AlarmStateNotificationManager : WeixinNotificationBaseManager, IAlarmStateNotificationManager
    {
        private readonly IRepository<State, long> stateRepository;
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly ILocalizationManager localizationManager;

        public AlarmStateNotificationManager(IRepository<State, long> stateRepository, IRepository<Machine> machineRepository, IRepository<MachineDeviceGroup> machineDeviceGroupRepository, IRepository<NotificationRule> notificationRuleRepository, IRepository<NotificationRuleDetail> notificationRuleDetailRepository, IRepository<User, long> useRepository,
            ILocalizationManager localizationManager)
            : base(useRepository, notificationRuleRepository, notificationRuleDetailRepository)
        {
            this.stateRepository = stateRepository;
            this.machineRepository = machineRepository;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.localizationManager = localizationManager;
        }

        public async Task<IEnumerable<State>> ListAlarmingStatesMachine()
        {
            var query = (from s in this.stateRepository.GetAll()
                         join m in this.machineRepository.GetAll() on s.MachineId equals m.Id
                         join d in this.machineDeviceGroupRepository.GetAll() on s.MachineId equals d.MachineId
                         where s.EndTime == null && s.Code == EnumMachineState.Stop.ToString()
                         select new { s.MachineId, MachineName = m.Name, s.StartTime, GroupId = d.DeviceGroupId }).ToList();

            // 数据去重 
            var stateList = query.GroupBy(
                p => p.MachineId,
                (key, g) => new { MachineId = key, State = g.OrderByDescending(p => p.StartTime).FirstOrDefault() });

            var result = stateList.Select(
                s => new State
                         {
                             MachineGroupInfo =
                                 new MachineGroupInfo()
                                     {
                                         MachineName = s.State.MachineName,
                                         GroupId = s.State.GroupId
                                     },
                             MachineId = s.MachineId,
                             Duration = (decimal)Math.Round((DateTime.Now - s.State.StartTime).Value.TotalMinutes, 2)
                         });
            return await Task.FromResult(result);
        }

        protected override string GetTemplate()
        {
            return localizationManager.GetString(BtlCoreConsts.LocalizationSourceName, "AlarmMessageTemplate");
        }

        protected override WeixinMessageDataText GetContent(string template, object data)
        {
            var states = data as WeixinMessageDataQueue<State>;
            return states == null
                       ? null
                       : new WeixinMessageDataText(
                           string.Format(template, states.Value.MachineGroupInfo.MachineName, states.Value.Duration));
        }

        protected override async Task<IEnumerable<NameValue<WeixinMessageDataDto>>> ParsingMessage(string agentid)
        {
            var messageData = await this.ListAlarmingStatesMachine();
            var rule = await this.ParsingMessageForwardingRules(messageData);

            return rule.Select(
                r => new NameValue<WeixinMessageDataDto>()
                         {
                             Name = r.Key,
                             Value = new WeixinMessageDataDto(
                                 this.GetContent(this.GetTemplate(), r),
                                 r.ToUser,
                                 agentid)
                         });
        }

        private async Task<IEnumerable<WeixinMessageDataQueue<State>>> ParsingMessageForwardingRules(IEnumerable<State> input)
        {
            var ruleList = this.GetNotificationRule(EnumMessageType.DeviceAlarm);
            var accountList = this.GetUserWeixinAccount();
            var stateList = new List<WeixinMessageDataQueue<State>>();

            foreach(var p in input)
            {
                var rule = ruleList
                    .Where(
                        n => n.Name.Contains(p.MachineGroupInfo.GroupId.ToString())
                             && p.Duration >= n.Value.TriggerCondition).OrderByDescending(n => n.Value.TriggerCondition)
                    .FirstOrDefault();
                if (rule == null) continue;

                var toUser = accountList
                    .Where(m => rule.Value.NoticeUserIds.Contains(m.Name) && !m.Value.IsNullOrEmpty())
                    .Select(u => u.Value).JoinAsString("|");

                if (!toUser.IsNullOrEmpty())
                    stateList.Add(
                        new WeixinMessageDataQueue<State>()
                        {
                            ToUser = toUser,
                            Value = p,
                            Key = $"{p.MachineId}#{rule.Value.TriggerCondition}"
                        });
            }


            return await Task.FromResult(stateList);
        }
    }
}