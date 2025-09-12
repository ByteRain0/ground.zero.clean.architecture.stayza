using Microsoft.Extensions.Logging;
using NSubstitute;
using Stayza.Core.Telemetry.LoggingAdapter;
using Stayza.Domain.Users;

namespace Stayza.Tests.Unit.Users;

public class EntitlementsServiceShould
{
    [Fact(Skip = "Ignored to showcase how logger won't work.")]
    public void Log_userId_when_evaluating()
    {
        // Arrange
        var logger = Substitute.For<ILogger<EntitlementService>>();
        var sut = new EntitlementService(logger);
        var user = new User("user-1");
        
        // Act
        sut.CanUserLoan(user);

        // Assert
        logger.Received(1).LogInformation("Checking entitlements for user with id : {userId}", Arg.Any<object?[]>());
    }
    
    [Fact]
    public void Log_userId_when_evaluating_v2()
    {
        // Arrange
        var logger = Substitute.For<ILoggerAdapter<EntitlementServiceV2>>();
        var sut = new EntitlementServiceV2(logger);
        var user = new User("user-1");
        
        // Act
        sut.CanUserLoan(user);

        // Assert
        logger.Received(1)
            .LogInformation("Checking entitlements for user with id : {userId}", Arg.Any<object?[]>());
    }

    [Fact]
    public void Log_exact_userId_when_evaluating_v2()
    {
        // Arrange
        var logger = Substitute.For<ILoggerAdapter<EntitlementServiceV2>>();
        var sut = new EntitlementServiceV2(logger);
        var user = new User("user-1");
        
        // Act
        sut.CanUserLoan(user);

        // Assert
        logger.Received(1).LogInformation("Checking entitlements for user with id : {userId}", 
            Arg.Is<object[]>(x => x[0].ToString() == user.Id));

    }
}