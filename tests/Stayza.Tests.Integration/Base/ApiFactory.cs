using System.Data.Common;
using System.Diagnostics;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Respawn;
using Stayza.Infrastructure.Persistence;
using Stayza.Web;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Stayza.Tests.Integration.Base;

public class ApiFactory : WebApplicationFactory<IWebMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .Build();    
    
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
        .WithPassword("guest")
        .WithUsername("guest")
        .WithImage("rabbitmq:3.11")
        .Build();
    
    private NpgsqlConnection _dbConnection = default!;
    
    private Respawner _respawner = default!;
    
    public RabbitMqTestMessageConsumer MessageConsumer;
    
    public HttpClient HttpClient = default!;
    
    public NotificationsApiServer NotificationsApi { get; } = new();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Un-register current EF core setup.
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            // Register EF Core with Test container
            services.AddDbContext<ApplicationDbContext>(opts => opts.UseNpgsql(_postgreSqlContainer.GetConnectionString()));
        });
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Way easier to just wire it in here :P
        Environment.SetEnvironmentVariable("OpenTelemetrySettings__Enabled", "false");
        Environment.SetEnvironmentVariable("RabbitMQSettings__ConnectionString", _rabbitMqContainer.GetConnectionString());
        return base.CreateHost(builder);
    }
    
    public async Task InitializeAsync()
    {
        await NotificationsApi.StartAsync();
        await _postgreSqlContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        MessageConsumer = new RabbitMqTestMessageConsumer(_rabbitMqContainer.GetConnectionString());
        HttpClient = GetEnrichedApiClient();
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
    
    private HttpClient GetEnrichedApiClient()
    {
        var httpClient = CreateClient();
        
        if (Activity.Current is null)
        {
            return httpClient;
        }
        
        var contextToInject = Activity.Current?.Context ?? default;
        
        OtelTestFramework.Propagator.Inject(
            new PropagationContext(contextToInject, Baggage.Current),
            httpClient,
            InjectTraceContext);
        
        return httpClient;
        
        static void InjectTraceContext(HttpClient client, string key, string value)
            => client.DefaultRequestHeaders.Add(key, new[] {value});
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