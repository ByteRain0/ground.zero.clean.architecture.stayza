using Stayza.Core.Entity;
using Stayza.Domain.BookCopyAggregate;

namespace Stayza.Domain.UserAggregate;

public class User : AggregateRoot
{
    public string EmailAddress { get; private set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public UserType UserType { get; set; }

    public bool IsActive { get; set; }
        = true;

    public List<Loan> ExistingLoans { get; private set; } = new();

    [Obsolete("Used only by ef core")]
    public User()
    {
    }
    
    public User(
        string firstName,
        string lastName,
        string emailAddress,
        Guid id) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
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