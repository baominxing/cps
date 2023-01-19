using Abp;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Notifications;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.Notifications.Dto;

namespace Wimi.BtlCore.Notifications
{
    [AbpAuthorize]
    public class NotificationAppService : BtlCoreAppServiceBase, INotificationAppService
    {
        private readonly INotificationDefinitionManager notificationDefinitionManager;

        private readonly INotificationSubscriptionManager notificationSubscriptionManager;

        private readonly IUserNotificationManager userNotificationManager;

        private readonly INotificationService notificationService;

        private readonly IRepository<NotificationRule> notificationRuleRepository;

        private readonly IRepository<NotificationRuleDetail> notificationRuleDetailRepository;

        private readonly IRepository<NotificationRecord> notificationRecordRepository;

        private readonly IRepository<DeviceGroup> deviceGroupRecordRepository;

        private readonly IRepository<User, long> userRepository;

        private readonly IRepository<ShiftSolution> shiftSolutionRepository;

        private readonly IRepository<ShiftSolutionItem> shiftSolutionItemsRepository;

        public NotificationAppService(
            INotificationDefinitionManager notificationDefinitionManager,
            IUserNotificationManager userNotificationManager,
            INotificationSubscriptionManager notificationSubscriptionManager,
            INotificationService notificationRuleManager,
            IRepository<NotificationRule> notificationRuleRepository,
            IRepository<NotificationRuleDetail> notificationRuleDetailRepository,
            IRepository<NotificationRecord> notificationRecordRepository,
            IRepository<DeviceGroup> deviceGroupRecordRepository,
            IRepository<User, long> userRecordRepository,
            IRepository<ShiftSolution> shiftSolutionRepository,
            IRepository<ShiftSolutionItem> shiftSolutionItemsRepository)
        {
            this.notificationDefinitionManager = notificationDefinitionManager;
            this.userNotificationManager = userNotificationManager;
            this.notificationSubscriptionManager = notificationSubscriptionManager;
            this.notificationService = notificationRuleManager;
            this.notificationRuleRepository = notificationRuleRepository;
            this.notificationRuleDetailRepository = notificationRuleDetailRepository;
            this.notificationRecordRepository = notificationRecordRepository;
            this.deviceGroupRecordRepository = deviceGroupRecordRepository;
            this.userRepository = userRecordRepository;
            this.shiftSolutionRepository = shiftSolutionRepository;
            this.shiftSolutionItemsRepository = shiftSolutionItemsRepository;
        }

        [HttpPost]
        public async Task<GetNotificationSettingsOutputDto> GetNotificationSettings()
        {
            var output = new GetNotificationSettingsOutputDto();

            output.ReceiveNotifications =
                await this.SettingManager.GetSettingValueAsync<bool>(NotificationSettingNames.ReceiveNotifications);

            var query = (await this.notificationDefinitionManager.GetAllAvailableAsync(this.AbpSession.ToUserIdentifier()))
                    .Where(nd => nd.EntityType == null); // Get general notifications, not entity related notifications.

            output.Notifications = ObjectMapper.Map<List<NotificationSubscriptionWithDisplayNameDto>>(query);

            var subscribedNotifications =
                (await
                 this.notificationSubscriptionManager.GetSubscribedNotificationsAsync(
                     this.AbpSession.ToUserIdentifier())).Select(ns => ns.NotificationName).ToList();

            output.Notifications.ForEach(n => n.IsSubscribed = subscribedNotifications.Contains(n.Name));

            return output;
        }

        [DisableAuditing]
        [HttpPost]
        public async Task<GetNotificationsOutputDto> GetUserNotifications(GetUserNotificationsInputDto input)
        {
            var totalCount =
                await
                this.userNotificationManager.GetUserNotificationCountAsync(
                    this.AbpSession.ToUserIdentifier(),
                    input.State);

            var unreadCount =
                await
                this.userNotificationManager.GetUserNotificationCountAsync(
                    this.AbpSession.ToUserIdentifier(),
                    UserNotificationState.Unread);

            var notifications =
                await
                    this.userNotificationManager.GetUserNotificationsAsync(
                        this.AbpSession.ToUserIdentifier(),
                        input.State);

            return new GetNotificationsOutputDto(totalCount, unreadCount, notifications);
        }

        public async Task SetAllNotificationsAsRead()
        {
            await
                this.userNotificationManager.UpdateAllUserNotificationStatesAsync(
                    this.AbpSession.ToUserIdentifier(),
                    UserNotificationState.Read);
        }

        public async Task SetNotificationAsRead(EntityDto<Guid> input)
        {
            var userNotification =
                await this.userNotificationManager.GetUserNotificationAsync(this.AbpSession.TenantId, input.Id);
            if (userNotification.UserId != this.AbpSession.GetUserId())
            {
                throw new ApplicationException(
                    this.L("GivenUserNotificationIdNotBelonged{0}{1}", input.Id, this.AbpSession.GetUserId()));
            }

            await
                this.userNotificationManager.UpdateUserNotificationStateAsync(
                    this.AbpSession.TenantId,
                    input.Id,
                    UserNotificationState.Read);
        }

        public async Task UpdateNotificationSettings(UpdateNotificationSettingsInputDto input)
        {
            await
                this.SettingManager.ChangeSettingForUserAsync(
                    this.AbpSession.ToUserIdentifier(),
                    NotificationSettingNames.ReceiveNotifications,
                    input.ReceiveNotifications.ToString());

            foreach (var notification in input.Notifications)
            {
                if (notification.IsSubscribed)
                {
                    await
                        this.notificationSubscriptionManager.SubscribeAsync(
                            this.AbpSession.ToUserIdentifier(),
                            notification.Name);
                }
                else
                {
                    await
                        this.notificationSubscriptionManager.UnsubscribeAsync(
                            this.AbpSession.ToUserIdentifier(),
                            notification.Name);
                }
            }
        }

        public async Task<IEnumerable<GetNotificationRuleDto>> ListNotificationRule()
        {
            var notificationRuleList = (from n in await this.notificationRuleRepository.GetAll().ToListAsync()
                                        select new GetNotificationRuleDto
                                        {
                                            Id = n.Id,
                                            Name = n.Name,
                                            DeviceGroupIds = n.DeviceGroupIds,
                                            DeviceGroupNames = this.deviceGroupRecordRepository.GetAll().ToList().Where(s => n.GetDeviceGroupIds().Any(ng => ng == s.Id.ToString())).Select(s => s.DisplayName).ToArray().JoinAsString(","),
                                            MessageType = n.MessageType,
                                            TriggerType = n.TriggerType,
                                            MemberCount = this.notificationRuleDetailRepository.GetAll().Count(s => s.NotificationRuleId == n.Id),
                                        }).ToList();

            return notificationRuleList;
        }

        public async Task<IEnumerable<int>> ListReferencedDeviceGroupId(NotificationRuleInputDto input)
        {
            var referencedDeviceGroupIds = new List<int>();

            var deviceGroupIdsList =
                from n in (await this.notificationRuleRepository.GetAllListAsync()).WhereIf(
                     input.Id != 0,
                     s => s.Id != input.Id)
                where n.MessageType == input.MessageType
                select n.DeviceGroupIds.Split(',');

            foreach (var deviceGroupIds in deviceGroupIdsList)
            {
                foreach (var deviceGroupId in deviceGroupIds)
                {
                    referencedDeviceGroupIds.Add(Convert.ToInt32(deviceGroupId));
                }
            }

            return referencedDeviceGroupIds;
        }

        [HttpPost]
        public async Task<GetNotificationRuleDto> GetNotificationRule(NotificationRuleInputDto input)
        {
            var notificationRule = await this.notificationRuleRepository.GetAsync(input.Id);

            var notificationRuleDto = ObjectMapper.Map<GetNotificationRuleDto>(notificationRule);
            notificationRuleDto.DeviceGroupIds = notificationRule.DeviceGroupIds;

            return notificationRuleDto;
        }

        public async Task<GetNotificationRuleDto> CreateNotificationRule(NotificationRuleInputDto input)
        {
            await this.notificationService.NotificationRuleNameIsExist(input.Id, input.Name);

            var notificationRule = new NotificationRule();

            notificationRule = ObjectMapper.Map<NotificationRule>(input);

            notificationRule.TriggerType = input.MessageType == EnumMessageType.YieldStatistics
                                               ? EnumTriggerType.TriggerWithShift
                                               : EnumTriggerType.TriggerWithTime;

            await this.notificationRuleRepository.InsertAsync(notificationRule);

            var notificationRuleDto = ObjectMapper.Map<GetNotificationRuleDto>(notificationRule);

            notificationRuleDto.DeviceGroupNames = this.deviceGroupRecordRepository.GetAll().ToList()
                .Where(s => notificationRule.GetDeviceGroupIds().Any(ng => ng == s.Id.ToString()))
                .Select(s => s.DisplayName).ToArray().JoinAsString(",");

            notificationRuleDto.MemberCount = this.notificationRuleDetailRepository.GetAll()
                .Count(s => s.NotificationRuleId == notificationRule.Id);

            return notificationRuleDto;
        }

        public async Task<GetNotificationRuleDto> UpdateNotificationRule(NotificationRuleInputDto input)
        {
            await this.notificationService.NotificationRuleNameIsExist(input.Id, input.Name);

            var notificationRule = await this.notificationRuleRepository.GetAsync(input.Id);

            //input.MapTo(notificationRule);
            ObjectMapper.Map(input, notificationRule);

            await this.notificationRuleRepository.UpdateAsync(notificationRule);

            var notificationRuleDto = ObjectMapper.Map<GetNotificationRuleDto>(notificationRule);

            notificationRuleDto.DeviceGroupNames = this.deviceGroupRecordRepository.GetAll().ToList()
                .Where(s => notificationRule.GetDeviceGroupIds().Any(ng => ng == s.Id.ToString()))
                .Select(s => s.DisplayName).ToArray().JoinAsString(",");

            notificationRuleDto.MemberCount = this.notificationRuleDetailRepository.GetAll()
                .Count(s => s.NotificationRuleId == notificationRule.Id);

            return notificationRuleDto;
        }

        public async Task DeleteNotificationRule(NotificationRuleInputDto input)
        {
            await this.notificationService.DeleteNotificationRule(input.Id);
        }
        public async Task<IEnumerable<GetNotificationRuleDetailDto>> ListNotificationRuleDetail(NotificationRuleInputDto input)
        {
            List<GetNotificationRuleDetailDto> notificationRuleDetailList;

            var notificationRule = await this.notificationRuleRepository.GetAsync(input.Id);
            ObjectMapper.Map(notificationRule, input);
            // notificationRule.MapTo(input);

            //ObjectMapper.Map(input, notificationRule);

            if (input.TriggerType == EnumTriggerType.TriggerWithShift)
            {
                notificationRuleDetailList =
                    (from n in await this.notificationRuleDetailRepository.GetAll().ToListAsync()
                     join ss in await this.shiftSolutionRepository.GetAllListAsync() on n.ShiftSolutionId equals ss.Id
                     join si in await this.shiftSolutionItemsRepository.GetAllListAsync() on n.ShiftId equals si.Id
                     where n.NotificationRuleId == input.Id
                     select new GetNotificationRuleDetailDto
                     {
                         Id = n.Id,
                         NotificationRuleId = n.NotificationRuleId,
                         TriggerCondition = n.TriggerCondition,
                         ShiftInfoName =
                                        input.TriggerType == EnumTriggerType
                                            .TriggerWithShift
                                            ? ss.Name + "-" + si.Name
                                            : string.Empty,
                         IsEnabled = n.IsEnabled,
                         NoticeUserIds = n.NoticeUserIds,
                         NoticeUserNames = this.userRepository.GetAll().ToList()
                                        .Where(
                                            s => n.GetNoticeUserIds().Any(
                                                ng => ng == s.Id.ToString()))
                                        .Select(s => s.Name).ToArray().JoinAsString(","),
                     }).OrderBy(s => s.TriggerCondition).ToList();
            }
            else
            {
                notificationRuleDetailList =
                    (from n in await this.notificationRuleDetailRepository.GetAll().ToListAsync()
                     where n.NotificationRuleId == input.Id
                     select new GetNotificationRuleDetailDto
                     {
                         Id = n.Id,
                         NotificationRuleId = n.NotificationRuleId,
                         TriggerCondition = n.TriggerCondition,
                         IsEnabled = n.IsEnabled,
                         NoticeUserIds = n.NoticeUserIds,
                         NoticeUserNames = this.userRepository.GetAll().ToList()
                                        .Where(
                                            s => n.GetNoticeUserIds().Any(
                                                ng => ng == s.Id.ToString()))
                                        .Select(s => s.Name).ToArray().JoinAsString(","),
                     }).OrderBy(s => s.TriggerCondition).ToList();
            }

            return notificationRuleDetailList;
        }

        [HttpPost]
        public async Task<GetNotificationRuleDetailDto> GetNotificationRuleDetail(NotificationRuleDetailInputDto input)
        {
            var notificationRuleDetail = await this.notificationRuleDetailRepository.GetAsync(input.Id);

            var notificationRuleDetailDto = ObjectMapper.Map<GetNotificationRuleDetailDto>(notificationRuleDetail);

            notificationRuleDetailDto.NoticeUserIds = notificationRuleDetail.NoticeUserIds;

            return notificationRuleDetailDto;
        }

        public async Task<GetNotificationRuleDetailDto> CreateNotificationRuleDetail(NotificationRuleDetailInputDto input)
        {
            await this.notificationService.TriggerConditionIsExist(
                input.NotificationRuleId,
                input.Id,
                input.TriggerCondition);

            var notificationRuleDetail = ObjectMapper.Map<NotificationRuleDetail>(input);

            //input.MapTo(notificationRuleDetail);

            await this.notificationRuleDetailRepository.InsertAsync(notificationRuleDetail);

            var notificationRuleDetailDto = ObjectMapper.Map<GetNotificationRuleDetailDto>(notificationRuleDetail);

            notificationRuleDetailDto.NoticeUserNames = this.userRepository.GetAll().ToList()
                .Where(s => notificationRuleDetail.GetNoticeUserIds().Any(ng => ng == s.Id.ToString())).Select(s => s.Name).ToArray()
                .JoinAsString(",");

            return notificationRuleDetailDto;
        }

        public async Task<GetNotificationRuleDetailDto> UpdateNotificationRuleDetail(NotificationRuleDetailInputDto input)
        {
            await this.notificationService.TriggerConditionIsExist(
                input.NotificationRuleId,
                input.Id,
                input.TriggerCondition);

            var notificationRuleDetail = await this.notificationRuleDetailRepository.GetAsync(input.Id);

            ObjectMapper.Map(input, notificationRuleDetail);

            await this.notificationRuleDetailRepository.UpdateAsync(notificationRuleDetail);

            var notificationRuleDetailDto = ObjectMapper.Map<GetNotificationRuleDetailDto>(notificationRuleDetail);

            notificationRuleDetailDto.NoticeUserNames = this.userRepository.GetAll().ToList()
                .Where(s => notificationRuleDetail.GetNoticeUserIds().Any(ng => ng == s.Id.ToString())).Select(s => s.Name).ToArray()
                .JoinAsString(",");

            return notificationRuleDetailDto;
        }

        public async Task DeleteNotificationRuleDetail(NotificationRuleDetailInputDto input)
        {
            await this.notificationRuleDetailRepository.DeleteAsync(s => s.Id == input.Id);
        }

        public async Task<IEnumerable<GetNotificationRecordDto>> ListNotificationRecord(NotificationRuleDetailInputDto input)
        {
            var notificationRuleDetailList = (from n in await this.notificationRecordRepository.GetAll().ToListAsync()
                                              where n.NoticedUserId == this.AbpSession.UserId
                                              && n.CreationTime >= input.StartTime && n.CreationTime < input.EndTime
                                              select new GetNotificationRecordDto
                                              {
                                                  Id = n.Id,
                                                  NotificationType = n.NotificationType.ToString(),
                                                  Status = n.Status.ToString(),
                                                  MessageType = n.MessageType.ToString(),
                                                  MessageContent = n.MessageContent,
                                                  CreationTime = n.CreationTime
                                              }).OrderBy(s => s.CreationTime).ToList();

            return notificationRuleDetailList;
        }

        [HttpPost]
        public async Task<IEnumerable<UserDto>> GetUserList()
        {
            return
                await
                    (from u in this.userRepository.GetAll()
                     select new UserDto() { UserId = u.Id, UserName = u.Name }).ToListAsync();
        }

        [HttpPost]
        public async Task<IEnumerable<GetNotificationRuleDetailDto>> GetReferencedShiftIds()
        {
            return
                await
                    (from u in this.notificationRuleDetailRepository.GetAll()
                     select new GetNotificationRuleDetailDto() { ShiftId = u.ShiftId }).ToListAsync();
        }

        public async Task<IEnumerable<NameValue<int>>> ListShiftSolution()
        {
            var shiftSolutionList = await
                (from ss in this.shiftSolutionRepository.GetAll()
                 select new NameValue<int>()
                 {
                     Name = ss.Name,
                     Value = ss.Id
                 }).ToListAsync();
            return shiftSolutionList;
        }

        public async Task<IEnumerable<NameValue<int>>> ListShift(NotificationRuleDetailInputDto input)
        {
            var referencedShiftIds = (from u in this.notificationRuleDetailRepository.GetAll()
                                      select u).WhereIf(input.Id != 0, s => input.Id != s.Id).Select(s => s.ShiftId).ToList();

            var shiftList = await
                (from si in this.shiftSolutionItemsRepository.GetAll()
                 where si.ShiftSolutionId == input.ShiftSolutionId
                 where !referencedShiftIds.Contains(si.Id)
                 select new NameValue<int>()
                 {
                     Name = si.Name,
                     Value = si.Id
                 }).ToListAsync();
            return shiftList;
        }
    }
}
