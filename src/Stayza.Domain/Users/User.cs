using Microsoft.AspNetCore.Identity;
using Stayza.Domain.Loans;

namespace Stayza.Domain.Users;

public class User : IdentityUser
{
    public UserType UserType { get; set; }

    public bool IsActive { get; set; }
        = true;

    public HashSet<Reservation> Reservations { get; private set; } = new();
    
    // TODO: return back the private setter once lesson is over.
    public HashSet<Loan> ExistingLoans { get; set; } = new();
    
    
    [Obsolete("Used only by ef core")]
    public User()
    {
    }
    
    public User(string id)
    {
        Id = id;
        UserType = UserType.General;
    }

    /// <summary>
    /// Homework: Introduce a hard limit to the humber of books a user can loan..
    /// If after reservation is made over limit cancell reservation.
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    public bool CanLoan(IEntitlementService service) => service.CanUserLoan(this);
}