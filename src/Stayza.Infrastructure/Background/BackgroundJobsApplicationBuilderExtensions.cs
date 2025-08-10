using Microsoft.Extensions.Hosting;
using Stayza.Infrastructure.Persistence;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;

namespace Stayza.Infrastructure.Background;

public static class BackgroundJobsApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddBackgroundJobs(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTickerQ(options =>
        {
            options.SetInstanceIdentifier("TickerQ");
            options.AddOperationalStore<ApplicationDbContext>(efOpt =>
            {
                efOpt.UseModelCustomizerForMigrations();
                efOpt.CancelMissedTickersOnApplicationRestart();
            });

            options.AddDashboard("/jobs");
            options.AddDashboardBasicAuth();
        });

        return builder;
    }
}