using Stayza.Domain.BookCopyAggregate;

namespace Stayza.Infrastructure.Notifications;

public class UserNotificationsService : IUserNotificationService
{
    public Task NotifyUser(Guid userId, string eventType)
    {
        return Task.CompletedTask;
    }
}