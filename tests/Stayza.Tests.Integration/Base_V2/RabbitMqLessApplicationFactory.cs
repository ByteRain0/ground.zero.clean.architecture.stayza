using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;
using RabbitMQ.Client;
using Respawn;
using Stayza.Core.Messaging;
using Stayza.Infrastructure.Messaging;
using Stayza.Infrastructure.Messaging.Subscriber;
using Stayza.Infrastructure.Persistence;
using Stayza.Tests.Integration.Base;
using Stayza.Tests.Integration.Mocks;
using Stayza.Web;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Stayza.Tests.Integration.Base_V2;

public class RabbitMqLessApplicationFactory :
    WebApplicationFactory<IWebMarker>,
    IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .Build();

    private Respawner _respawner = default!;

    private NpgsqlConnection _dbConnection = default!;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Overwrite the DBContext setup
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApplicationDbContext>));
            
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            
            services.AddDbContext<ApplicationDbContext>(opts => 
                opts.UseNpgsql(_postgreSqlContainer.GetConnectionString()));

            services.RemoveAll<IMessageProducer>();
            services.RemoveAll<IHostedService>();
            
            services.AddSingleton<MockEventsStore>();
            services.AddSingleton<IMessageProducer, MockProducer>();
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ExternalConfigurationOptions__Enabled", "false");
        return base.CreateHost(builder);
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
    }

    public async Task InitializeDbRespawner()
    {
        _dbConnection = new NpgsqlConnection(_postgreSqlContainer.GetConnectionString());
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public", "postgres", "library"]
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await using var connection = new NpgsqlConnection(_postgreSqlContainer.GetConnectionString());
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }
}