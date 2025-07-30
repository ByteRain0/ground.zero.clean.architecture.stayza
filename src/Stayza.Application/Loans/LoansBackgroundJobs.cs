using Microsoft.Extensions.Logging;
using Stayza.Domain.Loans;

namespace Stayza.Application.Loans;

public class LoansBackgroundJobs(
    ILoansRepository repository,
    IUserNotificationService userNotificationService,
    ILogger<LoansBackgroundJobs> logger,
    TimeProvider timeProvider)
{
    public async Task NotifyUsersAboutExpiringReservations()
    {
        var reservationsToExpire = await repository.GetReservationThatShouldExpire(
            after: timeProvider.GetUtcNow().AddDays(1),
            CancellationToken.None);

        foreach (var reservation in reservationsToExpire)
        {
            try
            {
                await userNotificationService.NotifyUser(reservation.UserId, "ReservationExpiresIn24H");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error encountered notifying user about reservation expiration. {userId}", reservation.UserId);
            }
        }
    }
    
    public async Task RemoveExpiredReservations()
    {
        try
        {
            await repository.RemoveExpiredAndCancelledReservations(timeProvider.GetUtcNow());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed removing expired and cancelled reservations");
            throw;
        }
    }
}