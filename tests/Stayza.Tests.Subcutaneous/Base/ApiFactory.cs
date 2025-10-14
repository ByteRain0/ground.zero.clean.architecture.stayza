using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Stayza.Core.Messaging;
using Stayza.Infrastructure.Messaging;
using Stayza.Infrastructure.Messaging.Subscriber;
using Stayza.Tests.Subcutaneous.Stubs;
using Stayza.Web;
using Testcontainers.PostgreSql;

namespace Stayza.Tests.Subcutaneous.Base;

public class ApiFactory : WebApplicationFactory<IWebMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // You can either introduce an enabled field into the appsettings and resolve like OTEL or use this approach.
            services.RemoveAll<IConnectionFactory>();
            services.RemoveAll<ModelFactory>();
            services.RemoveAll<IMessageProducer>();
            services.RemoveAll<RabbitMqReceiver>();
            services.RemoveAll<IListener>();
            services.RemoveAll<IHostedService>();
            services.AddSingleton<IMessageProducer, MessagePublisherStub>();
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("OpenTelemetrySettings__Enabled", "false");
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", _postgreSqlContainer.GetConnectionString());
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
}