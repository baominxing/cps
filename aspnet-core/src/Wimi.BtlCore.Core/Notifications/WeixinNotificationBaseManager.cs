namespace Wimi.BtlCore.Notifications
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Abp;
    using Abp.Application.Services.Dto;
    using Abp.Domain.Repositories;

    using Castle.Core.Internal;

    using Wimi.BtlCore.Authorization.Users;

    public abstract class WeixinNotificationBaseManager : IWeixinNotificationBaseManager
    {
        private readonly IRepository<User, long> useRepository;
        private readonly IRepository<NotificationRule> notificationRuleRepository;
        private readonly IRepository<NotificationRuleDetail> notificationRuleDetailRepository;

        protected WeixinNotificationBaseManager(IRepository<User, long> useRepository, IRepository<NotificationRule> notificationRuleRepository, IRepository<NotificationRuleDetail> notificationRuleDetailRepository)
        {
            this.useRepository = useRepository;
            this.notificationRuleRepository = notificationRuleRepository;
            this.notificationRuleDetailRepository = notificationRuleDetailRepository;
            this.MessageQueues = new List<WeixinMessageQueue>();
        }

        private IList<WeixinMessageQueue> MessageQueues { get; set; }

        public virtual async Task BuildMessageData(string agentid)
        {
            var type = this.GetMessageType();
            var messages = await this.ParsingMessage(agentid);
           
            var nameValues = messages as IList<NameValue<WeixinMessageDataDto>> ?? messages;
            this.FilterMessageQueue(nameValues, type);

            foreach(var m in nameValues)
            {
                if (!this.MessageQueues.Any(q => q.Key.Equals(m.Name) && q.Receiver.Equals(m.Value.touser) && q.Type == type))
                {
                    this.Add(
                        new WeixinMessageQueue()
                        {
                            Key = m.Name,
                            Receiver = m.Value.touser,
                            Message = m.Value,
                            Type = type
                        });
                }
            }
        }

        public async Task<IEnumerable<WeixinMessageDataDto>> Build(string agentid)
        {
            await this.BuildMessageData(agentid);
            return this.Get();
        }

        protected IEnumerable<NameValueDto<NotificationRuleDetail>> GetNotificationRule(EnumMessageType type)
        {
            return (from n in this.notificationRuleRepository.GetAll()
                    join d in this.notificationRuleDetailRepository.GetAll() on n.Id equals d.NotificationRuleId
                    where n.MessageType == type && d.IsEnabled
                    select new NameValueDto<NotificationRuleDetail>() { Name = n.DeviceGroupIds, Value = d }).ToList();
        }

        protected IEnumerable<NameValueDto<string>> GetUserWeixinAccount()
        {
            return this.useRepository.GetAll()
                .Select(u => new NameValueDto() { Name = u.Id.ToString(), Value = u.WeChatId }).ToList();
        }

        protected virtual void Add(WeixinMessageQueue input)
        {
            this.MessageQueues.Add(input);
        }

        protected abstract Task<IEnumerable<NameValue<WeixinMessageDataDto>>> ParsingMessage(string agentid);

        protected abstract string GetTemplate();

        protected abstract WeixinMessageDataText GetContent(string template, object data);

        private IEnumerable<WeixinMessageDataDto> Get()
        {
            return this.MessageQueues.Where(m => !m.Send).Select(m => m.Message).ToList();
        }

        private void FilterMessageQueue(IEnumerable<NameValue<WeixinMessageDataDto>> messages, EnumMessageType type)
        {
            var existNode = this.MessageQueues.Join(messages, q => q.Key, e => e.Name, (q, e) => q).Where(q => q.Type == type);
            var sendOutNodes = existNode as WeixinMessageQueue[] ?? existNode.ToArray();
            var removeNode = this.MessageQueues.Except(sendOutNodes).ToList();
            foreach (var item in removeNode)
            {
                var index = this.MessageQueues.IndexOf(item);
                this.MessageQueues.RemoveAt(index);
            }

            foreach(var e in sendOutNodes)
            {
                e.Send = true;
            }
        }

        private EnumMessageType GetMessageType()
        {
            var type = EnumMessageType.DeviceAlarm;

            if (this.GetType() == typeof(CutterWeixinNotificationManager))
            {
                type = EnumMessageType.ToolReminder;
            }

            if (this.GetType() == typeof(ShiftYieldNotificationManager))
            {
                type = EnumMessageType.YieldStatistics;
            }

            return type;
        }
    }
}