using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Respawn;
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
    
    public HttpClient HttpClient = default!;

    public string ExchangeName = "test_exchange";
    
    public NotificationsApiServer NotificationsApi { get; } = new();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // over-write DI services if needed.
        });
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("OpenTelemetrySettings__Enabled", "false");
        // Homework: Add a redis container and set it up to test cache.
        Environment.SetEnvironmentVariable("Cache__Enabled", "false");
        Environment.SetEnvironmentVariable("ExternalConfigurationOptions__Enabled", "false");
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", _postgreSqlContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("RabbitMQSettings__ConnectionString", _rabbitMqContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("RabbitMQSettings__ExchangeName", ExchangeName);
        Environment.SetEnvironmentVariable("Notifications__BaseUrl", NotificationsApi.Url);
        return base.CreateHost(builder);
    }
    
    public async Task InitializeAsync()
    {
        await NotificationsApi.StartAsync();
        await _postgreSqlContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        MessageConsumer = new RabbitMqTestMessageConsumer(_rabbitMqContainer.GetConnectionString());
        // Create a single client to be reused and at the same time trigger migration.
        HttpClient = CreateClient();
        await InitializeDbRespawner();
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
    
    private async Task InitializeDbRespawner()
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