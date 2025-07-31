using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stayza.Domain.Books;
using Stayza.Domain.Loans;
using Stayza.Infrastructure.Persistence.Extensions;
using Stayza.Infrastructure.Persistence.Repositories;

namespace Stayza.Infrastructure.Persistence;

public static class PersistenceApplicationBuilderExtensions
{
    internal static IHostApplicationBuilder AddPersistence(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<PublishDomainEventsInterceptor>();
        builder.Services.AddDbContext<ApplicationDbContext>(opts => 
            opts.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
        
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