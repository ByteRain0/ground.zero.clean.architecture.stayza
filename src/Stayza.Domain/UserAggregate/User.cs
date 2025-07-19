using Stayza.Core.Entity;
using Stayza.Domain.BookCopyAggregate;

namespace Stayza.Domain.UserAggregate;

public class User : Entity
{
    public EmailAddress EmailAddress { get; private set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public UserType UserType { get; set; }
    
    public bool IsActive { get; set; }

    public List<Loan> ExistingLoans { get; private set; } = new();

    public User(
        string firstName,
        string lastName,
        EmailAddress emailAddress,
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
    public bool CanBorrow(BorrowService service) => service.CanUserBorrow(this);
}