using Microsoft.EntityFrameworkCore;
using Stayza.Core.Messaging;
using Stayza.Infrastructure.Persistence;
using Stayza.Infrastructure.Persistence.Interceptors;
using Stayza.Infrastructure.Telemetry;
using Stayza.MigrationService.Stubs;
using Stayza.MigrationService.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.AddBaseTelemetryConfiguration();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
        tracing.AddSource(MigrationServiceRunTimeDiagnosticConfig.Source.Name));

builder.Services.AddSingleton<PublishDomainEventsInterceptor>();

builder.Services.AddSingleton<IMessageProducer, MessageProducerStub>();

builder.Services.AddDbContext<ApplicationDbContext>(opts => 
    opts.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddHostedService<DbMigrator>();

var host = builder.Build();
host.Run();