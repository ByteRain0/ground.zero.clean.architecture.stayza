using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stayza.Domain.Users;
using Stayza.Infrastructure.Persistence;

namespace Stayza.Infrastructure.Auth;

internal static class AuthApplicationBuilderExtensions
{
    // Full tutorial here : https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-9.0
    internal static IHostApplicationBuilder AddAuth(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        
        builder.Services
            .AddAuthorization()
            .AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);
        
        builder.Services.AddAuthorizationBuilder();

        builder.Services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        return builder;
    }
}