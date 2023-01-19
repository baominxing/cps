using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Abp;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Senparc.Weixin.CommonAPIs;
using Senparc.Weixin.Entities;
using Senparc.Weixin.QY.Containers;
using Wimi.BtlCore.Authorization;
using Wimi.BtlCore.Authorization.Users;
using Wimi.BtlCore.Configuration;
using Wimi.BtlCore.Dto;
using Wimi.BtlCore.Notifications;
using Wimi.BtlCore.WeChart;
using Wimi.BtlCore.Weixin.Dto;

namespace Wimi.BtlCore.Weixin
{
    public class WeixinAppService : BtlCoreAppServiceBase, IWeixinAppService
    {
        private readonly NotificationTypeManager notificationTypeManager;
        private readonly IRepository<NotificationType> notificationTypeRepository;
        private readonly IRepository<User, long> userRepository;
        private readonly IRepository<WeChatNotification> wechatNotificationRepositor;
        private readonly IWeixinNotificationProviderManager weixinNotificationProviderManager;

        public WeixinAppService(
            NotificationTypeManager notificationTypeManager,
            IRepository<NotificationType> notificationTypeRepository,
            IRepository<WeChatNotification> wechatNotificationReposity,
            IRepository<User, long> userRepository,
            IWeixinNotificationProviderManager weixinNotificationProviderManager)
        {
            this.notificationTypeManager = notificationTypeManager;
            this.notificationTypeRepository = notificationTypeRepository;
            this.wechatNotificationRepositor = wechatNotificationReposity;
            this.userRepository = userRepository;
            this.weixinNotificationProviderManager = weixinNotificationProviderManager;
        }

        /// <summary>
        /// 添加用户到通知类型
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [AbpAuthorize(PermissionNames.Pages_Administration_WeChatNotifications_ManageMembers)]
        public async Task AddMemberListToNotificationType(MemberListToNotificationTypeInputDto input)
        {
            foreach (var userId in input.MemberIdList)
            {
                var user = await this.userRepository.FirstOrDefaultAsync(q => q.Id == userId);
                if (user == null)
                {
                    throw new AbpException(this.L("ThereIsNoUserWithId{0}", userId));
                }

                var nt = await this.notificationTypeRepository.GetAsync(input.NotificationTypeId);

                var query = from uou in this.wechatNotificationRepositor.GetAll()
                            join ou in this.notificationTypeRepository.GetAll() on uou.NotificationTypeId equals ou.Id
                            where uou.UserId == user.Id
                            select ou;
                var currentOus = await Task.FromResult(query.ToList());

                if (currentOus.Any(cur => cur.Id == nt.Id))
                {
                    return;
                }

                await this.wechatNotificationRepositor.InsertAsync(new WeChatNotification(user.Id, nt.Id));
            }
        }

        /// <summary>
        /// 新建通知类型
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [AbpAuthorize(PermissionNames.Pages_Administration_WeChatNotifications_ManageNotificationTypes)]
        public async Task<NotificationTypeDto> CreateNotificationType(CreateNotificationTypeInputDto input)
        {
            var notificationType = new NotificationType(input.DisplayName);

            await this.notificationTypeManager.CreateAsync(notificationType);

            await this.CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<NotificationTypeDto>(notificationType);
        }

        /// <summary>
        /// 删除通知类型
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [AbpAuthorize(PermissionNames.Pages_Administration_WeChatNotifications_ManageNotificationTypes)]
        public async Task DeleteNotificationType(EntityDto input)
        {
            await this.notificationTypeRepository.DeleteAsync(input.Id);
            await this.wechatNotificationRepositor.DeleteAsync(t => t.NotificationTypeId == input.Id);
        }

        /// <summary>
        /// 获得不在该通知类型的用户
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<PagedResultDto<UserOutputDto>> FindUsers(FindUserInputDto input)
        {
            var exceptUserIds = new List<long>();
            if (input.NotificationTypeId.HasValue)
            {
                exceptUserIds =
                    this.wechatNotificationRepositor.GetAll()
                        .Where(g => g.NotificationTypeId == input.NotificationTypeId.Value)
                        .Select(q => q.UserId)
                        .ToList();
            }

            var query =
                this.userRepository.GetAll()
                    .WhereIf(input.NotificationTypeId.HasValue, m => exceptUserIds.All(g => g != m.Id))
                    .Where(q => q.IsActive)
                    .WhereIf(
                        !input.Search.Value.IsNullOrWhiteSpace(),
                        u =>
                        (u.Name).Contains(input.Search.Value) || u.UserName.Contains(input.Search.Value));

            var userCount = await query.CountAsync();
            var users = await query.OrderBy(u => u.Name).PageBy(input).ToListAsync();
            return new DatatablesPagedResultOutput<UserOutputDto>(
                userCount,
                users.Select(u => new UserOutputDto(u.Name, u.UserName, u.WeChatId, u.Id)).ToList(),
                userCount);
        }

        /// <summary>
        /// 获得通知类型
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        public async Task<ListResultDto<NotificationTypeDto>> GetNotificationTypes()
        {
            var query = from ou in this.notificationTypeRepository.GetAll()
                        join uou in this.wechatNotificationRepositor.GetAll() on ou.Id equals uou.NotificationTypeId
                            into g
                        select new { ou, memberCount = g.Count() };

            var items = await query.ToListAsync();

            return new ListResultDto<NotificationTypeDto>(
                items.Select(
                    item =>
                        {
                            var dto = ObjectMapper.Map<NotificationTypeDto>(item.ou);
                            dto.MemberCount = item.memberCount;
                            return dto;
                        }).ToList());
        }

        /// <summary>
        /// 获得该通知类型已有用户
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        public async Task<PagedResultDto<NotificationTypeUserDto>> GetNotificationTypeUsers(
            GetNotificationTypeUsersInputDto input)
        {
            var query = from uou in this.wechatNotificationRepositor.GetAll()
                        join ou in this.notificationTypeRepository.GetAll() on uou.NotificationTypeId equals ou.Id
                        join user in this.userRepository.GetAll() on uou.UserId equals user.Id
                        where input.Id == uou.NotificationTypeId
                        orderby input.Sorting
                        select new { uou, user };
            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync();
            var notificationTypeUserList = items.Select(
                item =>
                    {
                        var dto = ObjectMapper.Map<NotificationTypeUserDto>(item.uou);
                        dto.WeChatId = item.user.WeChatId;
                        dto.Name = item.user.Name;
                        return dto;
                    }).ToList();
            return new DatatablesPagedResultOutput<NotificationTypeUserDto>(
                totalCount,
                notificationTypeUserList);
        }

        #region 微信消息发送
        [HttpPost]
        public string GetToken()
        {
            return AccessTokenContainer.GetToken(AppSettings.WeixinYqConfig.WeixinCorpId, AppSettings.WeixinYqConfig.WeixinCorpSecret);
        }

        public void Send(WeixinMessageInputDto input)
        {
            try
            {
                var data = (WeixinMessageDataDto)input.Data;
                this.Logger.Info($"发送人：{data.touser} 内容：{data.text.content}");
                var result = CommonJsonSend.Send<WxJsonResult>(input.AccessToken, input.UrlFormat, input.Data);
                this.SetRecordStatus(result, input);
                this.Logger.Error("微信发送结果：" + result.errcode + result.errmsg);
            }
            catch (Exception ex)
            {
                this.Logger.Error("微信发送异常:" + ex);
            }
        }

        public async Task<IEnumerable<WeixinMessageDataDto>> ListWaitingMessageDatas()
        {
            return await this.weixinNotificationProviderManager.ListMessageDataDto(AppSettings.WeixinYqConfig.WeixinAgentid);
        }

        public async Task<IEnumerable<WeixinMessageDataDto>> ListShiftYieldDatas(int shiftSolutionItemId)
        {
            return await this.weixinNotificationProviderManager.ListShiftYieldData(AppSettings.WeixinYqConfig.WeixinAgentid, shiftSolutionItemId);
        }

        #endregion

        /// <summary>
        /// 从通知类型移除用户
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [AbpAuthorize(PermissionNames.Pages_Administration_WeChatNotifications_ManageMembers)]
        public async Task RemoveMemberFromNotificationType(MemberToNotificationTypeInputDto input)
        {
            var user = await this.userRepository.FirstOrDefaultAsync(u => u.Id == input.UserId);

            if (user == null)
            {
                throw new AbpException(this.L("ThereIsNoUserWithId{0}", input.UserId));
            }

            var nt = await this.notificationTypeRepository.GetAsync(input.NotificationTypeId);

            await this.wechatNotificationRepositor.DeleteAsync(u => u.UserId == user.Id && u.NotificationTypeId == nt.Id);
        }

        /// <summary>
        /// 更新通知类型
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [AbpAuthorize(PermissionNames.Pages_Administration_WeChatNotifications_ManageNotificationTypes)]
        public async Task<NotificationTypeDto> UpdateNotificationType(UpdateNotificationTypeInputDto input)
        {
            var notificationType = await this.notificationTypeRepository.GetAsync(input.Id);

            notificationType.DisplayName = input.DisplayName;

            await this.notificationTypeManager.ValidateNotificationTypeAsync(notificationType);

            await this.notificationTypeRepository.UpdateAsync(notificationType);
            var dto = ObjectMapper.Map<NotificationTypeDto>(notificationType);
            dto.MemberCount =
                await this.wechatNotificationRepositor.CountAsync(u => u.NotificationTypeId == notificationType.Id);
            return dto;
        }

        /// <summary>
        /// 更新微信通知是否启用
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task UpdateUserIsActive(EntityDto input)
        {
            var notification = await this.wechatNotificationRepositor.GetAsync(input.Id);
            if (notification != null)
            {
                notification.IsActive = !notification.IsActive;
            }
        }

        private void SetRecordStatus(WxJsonResult result, WeixinMessageInputDto input)
        {
            if (!result.errmsg.ToLower().Equals("ok"))
                return;
            var data = input.Data as WeixinMessageDataDto;
            if (data != null)
            {
                this.weixinNotificationProviderManager.UpdateRecordStatus(data.text.content);
            }
        }
    }
}