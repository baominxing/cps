namespace Wimi.BtlCore.Notifications
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Abp.Domain.Repositories;
    using Abp.Extensions;

    using Castle.Core.Internal;

    using Wimi.BtlCore.Authorization.Users;

    public class WeixinNotificationProviderManager : IWeixinNotificationProviderManager
    {
        private readonly IRepository<NotificationRecord> notificationRecordRepository;
        private readonly ICutterWeixinNotificationManager cutterWeixinNotificationManager;
        private readonly IAlarmStateNotificationManager alarmStateNotificationManager;
        private readonly IShiftYieldNotificationManager shiftYieldNotificationManager;
        private readonly IRepository<User, long> userRepository;

        public WeixinNotificationProviderManager(IRepository<NotificationRecord> notificationRecordRepository, ICutterWeixinNotificationManager cutterWeixinNotificationManager, IAlarmStateNotificationManager alarmStateNotificationManager, IShiftYieldNotificationManager shiftYieldNotificationManager, IRepository<User, long> userRepository)
        {
            this.notificationRecordRepository = notificationRecordRepository;
            this.cutterWeixinNotificationManager = cutterWeixinNotificationManager;
            this.alarmStateNotificationManager = alarmStateNotificationManager;
            this.shiftYieldNotificationManager = shiftYieldNotificationManager;
            this.userRepository = userRepository;
        }

        public async Task AddRecord(NotificationRecord record)
        {
            await this.notificationRecordRepository.InsertAsync(record);
        }

        public void UpdateRecordStatus(string content)
        {
            var records = this.notificationRecordRepository.GetAll().Where(n => n.MessageContent == content);
            foreach(var r in records)
            {
               r.Status = EnumNotificationRecordStatus.Undispatched;
            }
        }

        public async Task<IEnumerable<WeixinMessageDataDto>> ListMessageDataDto(string agentid)
        {
            var list = new List<WeixinMessageDataDto>();

            var alarmMessages = this.MergeMessageContent(await this.alarmStateNotificationManager.Build(agentid)).ToList();

            await this.CreateNotificationRecord(alarmMessages, EnumMessageType.DeviceAlarm);
            list.AddRange(alarmMessages);

            var cutterMessages = this.MergeMessageContent(await this.cutterWeixinNotificationManager.Build(agentid)).ToList();
            await this.CreateNotificationRecord(cutterMessages, EnumMessageType.ToolReminder);
            list.AddRange(cutterMessages);

            return list;
        }

        public async Task<IEnumerable<WeixinMessageDataDto>> ListShiftYieldData(string agentid, int shiftSolutionItemId)
        {
            var list = new List<WeixinMessageDataDto>();

            this.shiftYieldNotificationManager.SetShiftSolutionItemId(shiftSolutionItemId);
            var shiftMessages = this.MergeMessageContent(await this.shiftYieldNotificationManager.Build(agentid))
                .ToList();
            await this.CreateNotificationRecord(shiftMessages, EnumMessageType.YieldStatistics);
            list.AddRange(shiftMessages);

            return list;
        }

        private IEnumerable<WeixinMessageDataDto> MergeMessageContent(IEnumerable<WeixinMessageDataDto> input)
        {
            var groupByList = input.GroupBy(n => n.touser, (key, g) => new { ToUser = key, List = g.ToList() });
            var returnValue = new List<WeixinMessageDataDto>();

            foreach(var g in groupByList)
            {
                var content = string.Empty;
                var message = g.List.First();
                content = g.List.Aggregate(content, (current, item) => current + "\r\n" + item.text.content);
                returnValue.Add(
                    new WeixinMessageDataDto(
                        new WeixinMessageDataText(content.Trim('\r').Trim('\n')),
                        message.touser,
                        message.agentid));
            }

            return returnValue;
        }

        private async Task CreateNotificationRecord(IEnumerable<WeixinMessageDataDto> input, EnumMessageType type)
        {
            foreach (var item in input)
            {
                var wechartIds = item.touser.Split('|').Where(n => !n.IsNullOrWhiteSpace()).ToList();
                var users = this.userRepository.GetAll().Where(u => wechartIds.Contains(u.WeChatId));

                foreach (var user in users)
                {
                    var record = new NotificationRecord()
                    {
                        NotificationType = EnumNotificationType.WeChat,
                        MessageType = type,
                        Status = EnumNotificationRecordStatus.Dispatched,
                        MessageContent = item.text.content,
                        NoticedUserId = user.Id
                    };
                    await this.AddRecord(record);
                }
            }
        }
    }
}