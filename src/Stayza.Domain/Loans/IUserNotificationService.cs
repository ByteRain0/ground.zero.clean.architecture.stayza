namespace Stayza.Domain.Loans;

public interface IUserNotificationService
{
    /// <summary>
    /// Notify user about an event that happened in the system:
    /// Reservation: fulfilled, cancelled or expired.
    /// Loan: overdue.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="eventType"></param>
    /// <returns></returns>
    Task NotifyUser(
        string userId,
        string eventType);
}