using NSubstitute;
using Stayza.Domain.Users;

namespace Stayza.Tests.Unit.Users;

public class CanLoanShould
{
    [Fact]
    public void Returns_True_when_EntitlementService_allows_loan()
    {
        // Arrange
        var entitlementService = Substitute.For<IEntitlementService>();
        var user = new User("user-1");

        entitlementService.CanUserLoan(Arg.Any<User>()).Returns(true);

        // Act
        bool result = user.CanLoan(entitlementService);

        // Assert
        Assert.True(result);
        entitlementService.Received(1).CanUserLoan(user);
    }
}