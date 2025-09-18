using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stayza.Domain.Books;
using Stayza.Domain.Loans;
using Stayza.Infrastructure.Persistence.BackgroundJobs;
using Stayza.Infrastructure.Persistence.Interceptors;
using Stayza.Infrastructure.Persistence.Repositories;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;

namespace Stayza.Infrastructure.Persistence;

public static class PersistenceApplicationBuilderExtensions
{
    internal static IHostApplicationBuilder AddPersistence(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<PublishDomainEventsInterceptor>();
        
        builder.Services.AddDbContext<ApplicationDbContext>(opts => 
            opts.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
            
        builder.Services.AddTickerQ(options =>
        {
            // options.SetMaxConcurrency(4); // Max Concurrency for job execution
            // options.SetExceptionHandler<TickerExceptionHandler>(); // Exception handler for tickerq
            
            options.SetInstanceIdentifier("TickerQ");
            options.AddOperationalStore<ApplicationDbContext>(efOpt =>
            {
                efOpt.UseModelCustomizerForMigrations();
                efOpt.CancelMissedTickersOnApplicationRestart();
            });

            options.AddDashboard("/jobs");
            options.AddDashboardBasicAuth(); // Appsettings predefined
        });

        builder.Services
            .AddScoped<ILoansRepository, LoansRepository>()
            .AddScoped<IBooksRepository, BooksRepository>();
        
        return builder;
    }
    
    public static IApplicationBuilder ApplyDbMigrations(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
        context?.Database.Migrate();
        return app;
    }
}