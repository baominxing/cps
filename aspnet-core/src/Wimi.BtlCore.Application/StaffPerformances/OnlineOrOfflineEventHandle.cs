namespace Wimi.BtlCore.StaffPerformances
{
    using System.Linq;
    using System.Threading.Tasks;

    using Abp.Dependency;
    using Abp.Domain.Repositories;
    using Abp.Domain.Uow;
    using Abp.Events.Bus.Handlers;
    using Abp.Threading;

    using Wimi.BtlCore.StaffPerformance;

    public class OnlineOrOfflineEventHandle : IEventHandler<PersonOfflineEventData>,
                                              IEventHandler<PersonOnlineEventData>,
                                              ITransientDependency
    {
        private readonly IRepository<OnlineAndOfflineLog, long> onlineAndOfflineLogRepository;

        public OnlineOrOfflineEventHandle(IRepository<OnlineAndOfflineLog, long> onlineAndOfflineLogRepository)
        {
            this.onlineAndOfflineLogRepository = onlineAndOfflineLogRepository;
        }

        /// <summary>
        ///     人员上线事件处理
        /// </summary>
        /// <param name="eventData"></param>
        [UnitOfWork]
        public void HandleEvent(PersonOnlineEventData eventData)
        {
            AsyncHelper.RunSync(
                () => this.onlineAndOfflineLogRepository.InsertAsync(
                    new OnlineAndOfflineLog
                        {
                            UserId = eventData.UserId,
                            MachineId = eventData.DeviceId,
                            OnlineDateTime = eventData.OnlineDate,
                            OfflineDateTime = null
                        }));
        }

        /// <summary>
        ///     人员下线事件处理
        /// </summary>
        /// <param name="eventData"></param>
        [UnitOfWork]
        public void HandleEvent(PersonOfflineEventData eventData)
        {
            AsyncHelper.RunSync(
                () =>
                    {
                        var query = this.onlineAndOfflineLogRepository.GetAll()
                            .Where(q => q.MachineId == eventData.DeviceId)
                            .FirstOrDefault(q => q.OfflineDateTime == null);
                        if (query == null) return Task.FromResult((object)null);
                        query.OfflineDateTime = eventData.EventTime;
                        return this.onlineAndOfflineLogRepository.UpdateAsync(query);
                    });
        }
    }
}