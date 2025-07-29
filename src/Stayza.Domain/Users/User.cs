using Microsoft.AspNetCore.Identity;
using Stayza.Domain.Loans;

namespace Stayza.Domain.Users;

public class User : IdentityUser
{
    public UserType UserType { get; set; }

    public bool IsActive { get; set; }
        = true;

    public HashSet<Reservation> Reservations { get; private set; } = new();
    
    public HashSet<Loan> ExistingLoans { get; private set; } = new();
    
    
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
    /// Introduce a hard limit.
    /// If after reservation is made over limit cancell reservation.
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    public bool CanBorrow(EntitlementService service) => service.CanUserBorrow(this);
}