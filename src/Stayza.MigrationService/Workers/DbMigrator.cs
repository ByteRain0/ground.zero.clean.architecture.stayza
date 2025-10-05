using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Stayza.Core.Telemetry;
using Stayza.Infrastructure.Persistence;

namespace Stayza.MigrationService.Workers;

public class DbMigrator(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbMigrator>>();

        logger.LogInformation("Migrating database ...");
        using var applicationDbContextMigration = MigrationServiceRunTimeDiagnosticConfig.Source.StartActivity("Applying application Db context migrations", ActivityKind.Client);
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Activity.Current?.AddExceptionAndFail(ex);
            throw;
        }        
        applicationDbContextMigration?.Stop();
        
        // Note: Alternative approach if you have multiple DBCotnexts
        //await MigrateDbContext(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(), cancellationToken);

        hostApplicationLifetime.StopApplication();
    }

    private async Task MigrateDbContext(DbContext context, CancellationToken cancellationToken)
    {
        try
        {
            await context.Database.EnsureCreatedAsync(cancellationToken);
            await context.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Activity.Current?.AddExceptionAndFail(ex);
            throw;
        }  
    }
}