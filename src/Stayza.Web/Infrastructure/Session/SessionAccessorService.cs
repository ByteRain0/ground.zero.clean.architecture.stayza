using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Stayza.Domain.Users;

namespace Stayza.Web.Infrastructure.Session;

public class SessionAccessorService(
    SignInManager<User> signInManager, 
    ClaimsPrincipal claimsPrincipal)
{
    public async Task<string> GetUserId()
    {
        var userManager = signInManager.UserManager;
        if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            throw new InvalidOperationException("Invalid user");
        }
        
        return user.Id;
    }
}