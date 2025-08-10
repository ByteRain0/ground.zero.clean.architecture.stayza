using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Stayza.Domain.Users;

namespace Stayza.Infrastructure.Persistence.DataSeed;

public static class TestUserSeeder
{
    public const string TestUser1Email = "jhon.doe@example.com";
    public const string TestUser2Email = "jhoana.doe@example.com";
    
    public const string TestUser1Password = "Passw0rd!";
    public const string TestUser2Password = "Passw0rd!";
    
    public const string TestUser1Id = "bbb875a2-b427-4ee5-8957-e30343e108b6";
    public const string TestUser2Id = "cda77814-8cac-4a65-9bff-cd441cc63711";
    
    public static async Task SeedTestUsers(this IApplicationBuilder app)
    {
        await SeedUser(
            app: app, 
            userId: TestUser1Id,
            email: TestUser1Email,
            password: TestUser1Password);
        
        await SeedUser(
            app: app, 
            userId: TestUser2Id,
            email: TestUser2Email,
            password: TestUser2Password);
    }
    
    private static async Task SeedUser(
        IApplicationBuilder app,
        string userId,
        string email,
        string password)
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
        var userStore = serviceScope.ServiceProvider.GetRequiredService<IUserStore<User>>();

        var existingUser = await userStore.FindByIdAsync(
            userId: userId,
            cancellationToken: CancellationToken.None);
        
        if (existingUser is not null)
        {
            // User already exists.
            return;
        }
        
        var emailStore = (IUserEmailStore<User>)userStore;
        
        var user = new User
        {
            Id = userId
        };
        
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
            password: password);

        if (!result.Succeeded)
        {
            // If you decide to run this in live mode throwing a startup exception might be a bad idea.
            throw new InvalidOperationException("User creation failed");
        }
    }
}