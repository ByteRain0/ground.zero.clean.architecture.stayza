using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stayza.Core.Context;
using Stayza.Domain.BookCopyAggregate;
using Stayza.Infrastructure.Persistence;
using Stayza.Infrastructure.Persistence.Repositories;
using Stayza.Infrastructure.UserContext;

namespace Stayza.Infrastructure;

public static class AppllicationBuilderExtensions
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>();
        builder.Services.AddScoped<ILoansRepository, LoansRepository>();
        builder.Services.AddScoped<IUserContext, UserContextAccessor>();
        
        builder.Services.AddHttpContextAccessor();
        return builder;
    }
}