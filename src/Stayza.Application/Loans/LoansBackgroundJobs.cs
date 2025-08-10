using Microsoft.Extensions.Logging;
using Stayza.Domain.Loans;
using TickerQ.Utilities.Base;

namespace Stayza.Application.Loans;

public class LoansBackgroundJobs(
    ILoansRepository repository,
    IUserNotificationService userNotificationService,
    ILogger<LoansBackgroundJobs> logger,
    TimeProvider timeProvider)
{
    [TickerFunction(nameof(NotifyUsersAboutExpiringReservations), "0 7 * * *")]
    public async Task NotifyUsersAboutExpiringReservations()
    {
        var reservationsToExpire = await repository.GetReservationThatShouldExpire(
            after: timeProvider.GetUtcNow().AddDays(1),
            cancellationToken: CancellationToken.None);

        foreach (var reservation in reservationsToExpire)
        {
            try
            {
                await userNotificationService.NotifyUser(
                    userId: reservation.UserId,
                    eventType: "ReservationExpiresIn24H");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error encountered notifying user about reservation expiration. {userId}",
                    reservation.UserId);
            }
        }
    }
    
    [TickerFunction(nameof(RemoveExpiredReservations), "0 7 * * *")]
    public async Task RemoveExpiredReservations()
    {
        try
        {
            await repository.RemoveExpiredAndCancelledReservations(after: timeProvider.GetUtcNow());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed removing expired and cancelled reservations");
            throw;
        }
    }
}