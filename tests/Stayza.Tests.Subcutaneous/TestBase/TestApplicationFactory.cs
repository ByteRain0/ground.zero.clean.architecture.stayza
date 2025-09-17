using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stayza.Application.Books;
using Stayza.Core.Messaging;
using Stayza.Core.Telemetry.LoggingAdapter;
using Stayza.Domain.Books;
using Stayza.Domain.Users;
using Stayza.Infrastructure.Persistence;
using Stayza.Infrastructure.Persistence.Interceptors;
using Stayza.Infrastructure.Persistence.Repositories;
using Stayza.Tests.Subcutaneous.Stubs;

namespace Stayza.Tests.Subcutaneous.TestBase;

public class TestApplicationFactory 
    : IAsyncDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IServiceScope _scope;

    public TestApplicationFactory()
    {
        var services = new ServiceCollection();

        // Base
        services.AddLogging();
        services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
        
        // Domain
        services.AddScoped<IEntitlementService, EntitlementServiceV2>();
        
        // Application
        services.AddScoped<BooksService>();
        
        // Infrastructure
        services.AddScoped<IBooksRepository, BooksRepository>();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
        });
        services.AddSingleton<PublishDomainEventsInterceptor>();
        services.AddScoped<IMessageProducer, MessagePublisherStub>();
        
        _serviceProvider = services.BuildServiceProvider();
        _scope = _serviceProvider.CreateScope();
    }

    public TService GetService<TService>() where TService : notnull
    {
        return _scope.ServiceProvider.GetRequiredService<TService>();
    }

    public ValueTask DisposeAsync()
    {
        _scope.Dispose();
        return _serviceProvider.DisposeAsync();
    }
}