namespace Wimi.BtlCore.StaffPerformances
{
    using System;

    using Abp.Dependency;
    using Abp.Events.Bus.Handlers;
    using Abp.Localization;
    using Abp.UI;
    using Wimi.BtlCore.Machines.Mongo;
    using Wimi.BtlCore.StaffPerformance;

    public class PerformanceDeviceMongoEventHandler : 
        IEventHandler<PersonOfflineEventData>,
        IEventHandler<PersonOnlineEventData>, ITransientDependency
    {
        private readonly MongoMachineManager mongoMachineManager;
        private readonly ILocalizationManager localizationManager;

        public PerformanceDeviceMongoEventHandler(MongoMachineManager mongoMachineManager,
            ILocalizationManager localizationManager)
        {
            this.mongoMachineManager = mongoMachineManager;
            this.localizationManager = localizationManager;
        }

        /// <summary>
        /// 更新Mongo "Machine"Collection中的UserId 和 UserShiftDetailId
        /// </summary>
        /// <param name="eventData"></param>
        public void HandleEvent(PersonOnlineEventData eventData)
        {
            mongoMachineManager.UpdateMongoMachineUser(eventData.DeviceId, eventData.UserId, eventData.ShiftId,eventData.ShiftItemName);
        }

        /// <summary>
        /// 更新Mongo "Machine"Collection，置空UserId 和 UserShiftDetailId
        /// </summary>
        /// <param name="eventData"></param>
        public void HandleEvent(PersonOfflineEventData eventData)
        {
            try
            {
                mongoMachineManager.UpdateMongoMachineUser(eventData.DeviceId, 0, 0, string.Empty);
            }
            catch
            {
                throw new UserFriendlyException(localizationManager.GetString(BtlCoreConsts.LocalizationSourceName, "OperationFailed"));
            }
        }
    }
}
