using Microsoft.AspNetCore.Identity;
using Stayza.Domain.Users;

namespace Stayza.Web.Infrastructure.Seed;

public static class TestUserSeeder
{
    public const string TestUserEmail = "jhon.doe@example.com";
    public const string TestUserPassword = "Passw0rd!";
    public const string TestUserId = "bbb875a2-b427-4ee5-8957-e30343e108b6";
    
    public static async Task SeedTestUser(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
        var userStore = serviceScope.ServiceProvider.GetRequiredService<IUserStore<User>>();

        var existingUser = await userStore.FindByIdAsync(
            userId: TestUserId,
            cancellationToken: CancellationToken.None);

        if (existingUser is not null)
        {
            // User already exists.
            return;
        }
        
        var emailStore = (IUserEmailStore<User>)userStore;
        var email = TestUserEmail;
        
        var user = new User();
        await userStore.SetUserNameAsync(
            user: user, 
            userName: email,
            cancellationToken: CancellationToken.None);
        
        await emailStore.SetEmailAsync(
            user: user,
            email: email,
            cancellationToken: CancellationToken.None);
        
        var result = await userManager!.CreateAsync(
            user: user,
            password: TestUserPassword);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Test user creation failed");
        }
    }
}