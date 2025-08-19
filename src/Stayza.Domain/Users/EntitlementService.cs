namespace Stayza.Domain.Users;

/// <summary>
/// A DS is a service that contains only business logic.
/// Prefer keeping domain logic in the domain models where possible.
/// </summary>
public class EntitlementService
{
    public bool CanUserLoan(User user)
    {
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