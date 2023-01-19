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
using Wimi.BtlCore.Cutter;

namespace Wimi.BtlCore.Notifications
{
    public class CutterWeixinNotificationManager : WeixinNotificationBaseManager, ICutterWeixinNotificationManager
    {
        private readonly IRepository<Machine> machineRepository;
        private readonly IRepository<CutterStates> cutterStateRepository;
        private readonly IRepository<MachineDeviceGroup> machineDeviceGroupRepository;
        private readonly ILocalizationManager localizationManager;

        public CutterWeixinNotificationManager(
            IRepository<Machine> machineRepository,
            IRepository<CutterStates> cutterStateRepository,
            IRepository<MachineDeviceGroup> machineDeviceGroupRepository,
            IRepository<NotificationRule> notificationRuleRepository,
            IRepository<User, long> useRepository,
            IRepository<NotificationRuleDetail> notificationRuleDetailRepository,
            ILocalizationManager localizationManager)
            : base(useRepository, notificationRuleRepository, notificationRuleDetailRepository)
        {
            this.machineRepository = machineRepository;
            this.cutterStateRepository = cutterStateRepository;
            this.machineDeviceGroupRepository = machineDeviceGroupRepository;
            this.localizationManager = localizationManager;
        }

        public async Task<IEnumerable<CutterStates>> ListWarningCutters()
        {
            var query = (from s in this.cutterStateRepository.GetAll()
                         join m in this.machineRepository.GetAll() on s.MachineId equals m.Id
                         join d in this.machineDeviceGroupRepository.GetAll() on s.MachineId equals d.MachineId
                         where s.CutterUsedStatus == EnumCutterUsedStates.Loading
                         select new { Dto = s, MachineId = m.Id, MachineName = m.Name, GroupId = d.DeviceGroupId }).ToList();

            // 数据去重 (一个设备位于多个设备组)
            var cutterStateList = query.GroupBy(
                p => p.MachineId,
                (key, g) => new { MachineId = key, Cutter = g.FirstOrDefault() });

            var list = cutterStateList.Select(
                c => new CutterStates()
                {
                    CutterNo = c.Cutter.Dto.CutterNo,
                    RestLife = c.Cutter.Dto.RestLife,
                    MachineId = c.MachineId,
                    CutterTValue = c.Cutter.Dto.CutterTValue,
                    WarningLife = c.Cutter.Dto.WarningLife,
                    MachineGroupInfo =
                                 new MachineGroupInfo()
                                 {
                                     GroupId = c.Cutter.GroupId,
                                     MachineName = c.Cutter.MachineName
                                 }
                });

            return await Task.FromResult(list);
        }

        protected override string GetTemplate()
        {
            return localizationManager.GetString(BtlCoreConsts.LocalizationSourceName, "CutterAlarmTemplate");
        }

        protected override WeixinMessageDataText GetContent(string template, object data)
        {
            var cutter = data as WeixinMessageDataQueue<CutterStates>;
            if (cutter == null) throw new ArgumentNullException(nameof(cutter));

            return new WeixinMessageDataText(
                string.Format(
                    template,
                    cutter.Value.CutterNo,
                    cutter.Value.RestLife,
                    cutter.Value.MachineGroupInfo.MachineName,
                    cutter.Value.CutterTValue));
        }

        protected override async Task<IEnumerable<NameValue<WeixinMessageDataDto>>> ParsingMessage(string agentid)
        {
            var messageData = await this.ListWarningCutters();
            var rule = await this.ParsingMessageForwardingRules(messageData);
            return rule.Select(
                d => new NameValue<WeixinMessageDataDto>(
                    d.Key,
                    new WeixinMessageDataDto(this.GetContent(this.GetTemplate(), d), d.ToUser, agentid)));
        }

        private async Task<IEnumerable<WeixinMessageDataQueue<CutterStates>>> ParsingMessageForwardingRules(IEnumerable<CutterStates> input)
        {
            var ruleList = this.GetNotificationRule(EnumMessageType.ToolReminder);
            var cutterList = new List<WeixinMessageDataQueue<CutterStates>>();
            var accountList = this.GetUserWeixinAccount();

            foreach(var p in input)
            {
                var rule = ruleList
                    .Where(
                        n => n.Name.Contains(p.MachineGroupInfo.GroupId.ToString())
                             && n.Value.TriggerCondition >= p.RestLife)
                    .OrderBy(n => n.Value.TriggerCondition).FirstOrDefault();
                if (rule == null) continue;

                var toUser = accountList
                    .Where(m => rule.Value.NoticeUserIds.Contains(m.Name) && !m.Value.IsNullOrEmpty())
                    .Select(u => u.Value).JoinAsString("|");

                if (!toUser.IsNullOrEmpty())
                    cutterList.Add(
                        new WeixinMessageDataQueue<CutterStates>()
                        {
                            ToUser = toUser,
                            Value = p,
                            Key =
                                    $"{p.CutterNo}#{rule.Value.TriggerCondition}"
                        });
            }


            return await Task.FromResult(cutterList);
        }
    }
}
