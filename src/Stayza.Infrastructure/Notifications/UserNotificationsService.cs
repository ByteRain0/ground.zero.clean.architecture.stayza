using System.Net.Http.Json;
using Stayza.Domain.Loans;

namespace Stayza.Infrastructure.Notifications;

public class UserNotificationsService(HttpClient httpClient) : IUserNotificationService
{
    public async Task NotifyUser(string userId, string eventType)
    {
        var request = await httpClient.PostAsJsonAsync(
            requestUri: "api/v1/notifications", 
            value: new Notification(UserId: userId.ToString(), Type: eventType));

        if (!request.IsSuccessStatusCode)
        {
            // TODO: think if you really want to break the execution flow just because a notification failed? :P
            throw new Exception("Error sending notification to user");
        }
    }
    
    internal record Notification(
        string UserId,
        string Type);
}