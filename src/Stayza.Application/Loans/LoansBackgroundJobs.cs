using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stayza.Domain.Loans;
using TickerQ.Utilities.Base;

namespace Stayza.Application.Loans;

public class LoansBackgroundJobs(
    IServiceScopeFactory serviceScopeFactory,
    TimeProvider timeProvider)
{
    [TickerFunction(nameof(NotifyUsersAboutExpiringReservations), "0 7 * * *")]
    public async Task NotifyUsersAboutExpiringReservations()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<LoansBackgroundJobs>>();
        var repository = scope.ServiceProvider.GetRequiredService<ILoansRepository>();
        var userNotificationService = scope.ServiceProvider.GetRequiredService<IUserNotificationService>();

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

    [TickerFunction(nameof(RemoveExpiredAndCancelledReservations), "0 7 * * *")]
    public async Task RemoveExpiredAndCancelledReservations()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ILoansRepository>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<LoansBackgroundJobs>>();

        try
        {
            // One option is splitting this jobs in 2 parts
            // first that cancells the expired reservations and publishes the respective domain events
            // second that cleans up the expired reservations
            await repository.RemoveExpiredAndCancelledReservations(after: timeProvider.GetUtcNow());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed removing expired and cancelled reservations");
            throw;
        }
    }
}