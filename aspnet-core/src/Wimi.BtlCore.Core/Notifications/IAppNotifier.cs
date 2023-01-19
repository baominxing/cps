namespace Wimi.BtlCore.Notifications
{
    using System;
    using System.Threading.Tasks;

    using Abp;
    using Abp.Notifications;

    using Wimi.BtlCore.Authorization.Users;
    using Wimi.BtlCore.MultiTenancy;

    public interface IAppNotifier
    {
        Task NewTenantRegisteredAsync(Tenant tenant);

        Task NewUserRegisteredAsync(User user);

        Task SendMessageAsync(
            UserIdentifier user, 
            string message, 
            NotificationSeverity severity = NotificationSeverity.Info);

        Task WelcomeToTheApplicationAsync(User user);

        Task GdprDataPrepared(UserIdentifier user, Guid binaryObjectId);
    }
}