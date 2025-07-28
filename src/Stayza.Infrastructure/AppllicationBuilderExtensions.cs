using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stayza.Core.Context;
using Stayza.Domain.Books;
using Stayza.Domain.Loans;
using Stayza.Infrastructure.Messaging;
using Stayza.Infrastructure.Notifications;
using Stayza.Infrastructure.Persistence;
using Stayza.Infrastructure.Persistence.Repositories;
using Stayza.Infrastructure.UserContext;

namespace Stayza.Infrastructure;

public static class AppllicationBuilderExtensions
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
        
        builder.Services
            .AddScoped<ILoansRepository, LoansRepository>()
            .AddScoped<IBooksRepository, BooksRepository>()
            .AddScoped<IUserContext, UserContextAccessor>()
            .AddSingleton<TimeProvider>(TimeProvider.System)
            .AddHttpClient<IUserNotificationService, UserNotificationsService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["Notifications__URL"] 
                                             ?? throw new Exception("Invalid notifications api configuration provided"));
            });
        
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAsyncMessagingUsingRabbitMq(builder.Configuration);
        
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