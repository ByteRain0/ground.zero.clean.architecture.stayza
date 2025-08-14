using Microsoft.Extensions.Logging;
using Stayza.Domain.Loans;
using TickerQ.Utilities.Base;

namespace Stayza.Application.Loans;

public class LoansBackgroundJobs(
    ILogger<LoansBackgroundJobs> logger, 
    ILoansRepository loansRepository, 
    IUserNotificationService userNotificationService, 
    TimeProvider timeProvider)
{
    [TickerFunction(nameof(NotifyUsersAboutExpiringReservations), "0 7 * * *")]
    public async Task NotifyUsersAboutExpiringReservations()
    {
        
        var reservationsToExpire = await loansRepository.GetReservationThatShouldExpire(
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
    
    [TickerFunction(nameof(RemoveCancelledReservations), "0 5 * * *")]
    public async Task CancelExpiredReservations()
    {
        var expiredReservations = await loansRepository.GetExpiredReservations(after: timeProvider.GetUtcNow());

        foreach (var reservation in expiredReservations)
        {
            try
            {
                var bookCopy = await loansRepository.GetBookCopyById(
                    id: reservation.BookCopyId,
                    cancellationToken: CancellationToken.None);

                bookCopy.CancelReservation(
                    reservationId: reservation.Id,
                    reason: "EXPIRED");

                await loansRepository.UpdateBookCopy(bookCopy);
            }
            catch (Exception e)
            {
                // HW: add additional information to the spans :) to make it easier to debug later on.
                logger.LogError(e, "Failed cancelling expired reservation.");
            }
        }
    }
    
    [TickerFunction(nameof(RemoveCancelledReservations), "0 7 * * *")]
    public async Task RemoveCancelledReservations()
    {
        try
        {
            await loansRepository.RemoveCancelledReservation();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed removing cancelled reservations");
            throw;
        }
    }
}