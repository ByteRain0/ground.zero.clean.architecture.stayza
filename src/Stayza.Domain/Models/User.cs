namespace Stayza.Domain.Models;

public class User
{
    public Guid Id { get; private set; }

    public EmailAddress EmailAddress { get; private set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public UserType UserType { get; set; }
    
    public bool IsActive { get; set; }

    private const int MaxActiveLoans = 5;
    
    private readonly List<Guid> _activeLoanIds = new();
    
    public User(
        string firstName,
        string lastName,
        EmailAddress emailAddress,
        Guid? id)
    {
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        UserType = UserType.General;
        Id = id ?? Guid.NewGuid();
    }
    
    public bool CanBorrow()
    {
        return IsActive && _activeLoanIds.Count < MaxActiveLoans;
    }

    /// <summary>
    /// Think if there needs to be an event in here for the loan aggregate.
    /// </summary>
    /// <param name="loanId"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddLoan(Guid loanId)
    {
        if (!CanBorrow())
            throw new InvalidOperationException("User cannot borrow more books.");

        _activeLoanIds.Add(loanId);
    }
    
    /// <summary>
    /// Trigger here a domain event and pick up it in the loan aggregate.
    /// </summary>
    /// <param name="loanId"></param>
    public void ReturnLoan(Guid loanId)
    {
        _activeLoanIds.Remove(loanId);
    }

    public void Deactivate() => IsActive = false;

    public void Reactivate() => IsActive = true;
    
    
}