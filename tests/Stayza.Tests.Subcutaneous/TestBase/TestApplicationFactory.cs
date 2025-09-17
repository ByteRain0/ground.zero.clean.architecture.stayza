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
using Testcontainers.PostgreSql;

namespace Stayza.Tests.Subcutaneous.TestBase;

public class TestApplicationFactory 
    : IAsyncLifetime
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IServiceScope _scope;
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .Build();
    
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
        services.AddDbContext<ApplicationDbContext>(opts => 
            opts.UseNpgsql(_postgreSqlContainer.GetConnectionString()));
        services.AddSingleton<PublishDomainEventsInterceptor>();
        services.AddScoped<IMessageProducer, MessagePublisherStub>();
        
        _serviceProvider = services.BuildServiceProvider();
        _scope = _serviceProvider.CreateScope();
    }

    public TService GetService<TService>() where TService : notnull
    {
        return _scope.ServiceProvider.GetRequiredService<TService>();
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
        _scope.Dispose();
        await _serviceProvider.DisposeAsync();
    }
}