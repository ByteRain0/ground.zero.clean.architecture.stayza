using Shouldly;
using Stayza.Domain.Loans;
using Stayza.Domain.Users;
using Stayza.Tests.Subcutaneous.Base.TestConstants;

namespace Stayza.Tests.Subcutaneous.TestBase;

public class EntitlementServiceShould
{
    [Fact]
    public async Task Forbid_general_users_from_borrowing_more_than_5_books()
    {
        // Arrange
        var testFactory = new TestApplicationFactory();
        var user = new User(Guid.NewGuid().ToString());
        var loans = new List<Loan>();
        
        for (int i = 0; i < 5; i++)
        {
            loans.Add(new Loan(
                bookCopyId: Constants.BookCopy.BookCopyId,
                userId: user.Id,
                timeRange: new TimeRange(start:Constants.Time.TestUtcNow, 
                    end: Constants.Time.TestUtcNow.AddDays(30)),
                id: Guid.NewGuid()));
        }

        user.ExistingLoans = [..loans];
        
        var sut = testFactory.GetService<IEntitlementService>();
        
        // Act & Assert
        sut.CanUserLoan(user).ShouldBeFalse();
    }
}