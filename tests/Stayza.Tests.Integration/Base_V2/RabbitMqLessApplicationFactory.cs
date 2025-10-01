using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using RabbitMQ.Client;
using Respawn;
using Stayza.Infrastructure.Messaging;
using Stayza.Infrastructure.Persistence;
using Stayza.Tests.Integration.Base;
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

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
        .WithPassword("guest")
        .WithUsername("guest")
        .Build();

    private Respawner _respawner = default!;

    private NpgsqlConnection _dbConnection = default!;

    public RabbitMqTestMessageConsumer _consumer;
    
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

            
            services.RemoveAll<RabbitMQSettings>();
            services.AddSingleton(new RabbitMQSettings
            {
                ConnectionString = _rabbitMqContainer.GetConnectionString(),
                ExchangeName = "test_exchange",
                ExchangeType = "topic"
            });
            services.RemoveAll<IConnectionFactory>();
            services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
            {
                DispatchConsumersAsync = true,
                Uri = new Uri(_rabbitMqContainer.GetConnectionString())
            });     
        });
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        _consumer = new RabbitMqTestMessageConsumer(_rabbitMqContainer.GetConnectionString());
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
        await _rabbitMqContainer.StopAsync();
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