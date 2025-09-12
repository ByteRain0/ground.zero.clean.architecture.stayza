using Stayza.Core.Telemetry.LoggingAdapter;

namespace Stayza.Domain.Users;

public class EntitlementServiceV2 : IEntitlementService
{
    private readonly ILoggerAdapter<EntitlementServiceV2> _logger;

    public EntitlementServiceV2(ILoggerAdapter<EntitlementServiceV2> logger)
    {
        _logger = logger;
    }

    public bool CanUserLoan(User user)
    {
        _logger.LogInformation("Checking entitlements for user with id : {userId}", user.Id);
        
        var maxActiveLoans = user.UserType switch
        {
            UserType.General => 5,
            UserType.Premium => 10,
            _ => 0
        };

        var activeLoansCount = user.ExistingLoans
            .Count(x => x.IsReturned == false);

        return user.IsActive && activeLoansCount < maxActiveLoans;
    }
}