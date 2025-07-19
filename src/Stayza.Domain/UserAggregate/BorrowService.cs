namespace Stayza.Domain.UserAggregate;

public class BorrowService
{
    /// <summary>
    /// In the future you might want to access some external service to get the 5/10 count;
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public bool CanUserBorrow(User user)
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