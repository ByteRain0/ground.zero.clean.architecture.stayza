using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Stayza.Domain.Users;

namespace Stayza.Web.Infrastructure.Session;

internal class SessionAccessorService(
    SignInManager<User> signInManager)
{
    public async Task<string> GetUserId(ClaimsPrincipal claimsPrincipal)
    {
        var userManager = signInManager.UserManager;
        if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            throw new InvalidOperationException("Invalid user");
        }
        
        return user.Id;
    }
}