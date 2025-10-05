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
using Stayza.Infrastructure.Messaging;
using Stayza.Infrastructure.Persistence;
using Stayza.Web;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Stayza.Tests.Integration.Base;

public class ApiFactory : WebApplicationFactory<IWebMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .Build();    
    
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
        .WithPassword("guest")
        .WithUsername("guest")
        .Build();
    
    private NpgsqlConnection _dbConnection = default!;
    
    private Respawner _respawner = default!;
    
    public RabbitMqTestMessageConsumer MessageConsumer;
    
    public string ExchangeName = "test_notifications_exchange";
    
    public NotificationsApiServer NotificationsApi { get; } = new();
    
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

            // Overwrite the RabbitMq setup
            services.RemoveAll<IConnectionFactory>();
            services.RemoveAll<RabbitMQSettings>();
            services.AddSingleton(new RabbitMQSettings
            {
                ConnectionString = _rabbitMqContainer.GetConnectionString(),
                ExchangeName = ExchangeName,
                ExchangeType = "topic"
            });
            var connectionStringToRabbitMq = _rabbitMqContainer.GetConnectionString();
            services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
            {
                DispatchConsumersAsync = true,
                Uri = new Uri(connectionStringToRabbitMq)
            });            
        });
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("OpenTelemetrySettings__Enabled", "false");
        Environment.SetEnvironmentVariable("Cache__Enabled", "false");
        Environment.SetEnvironmentVariable("ExternalConfigurationOptions__Enabled", "false");
        Environment.SetEnvironmentVariable("Notifications__BaseUrl", NotificationsApi.Url);
        return base.CreateHost(builder);
    }
    
    public async Task InitializeAsync()
    {
        await NotificationsApi.StartAsync();
        await _postgreSqlContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        MessageConsumer = new RabbitMqTestMessageConsumer(_rabbitMqContainer.GetConnectionString());
    }

    public async Task DisposeAsync()
    {
        await NotificationsApi.DisposeAsync();
        await _postgreSqlContainer.StopAsync();
        await _rabbitMqContainer.StopAsync();
    }
    
    public async Task ResetDatabaseAsync()
    {
        await using var conn = new NpgsqlConnection(_postgreSqlContainer.GetConnectionString());
        await conn.OpenAsync();
        await _respawner.ResetAsync(conn);
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
}