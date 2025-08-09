using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Stayza.Web.Infrastructure.Seed;

namespace Stayza.Tests.Integration.Base;

public static class AuthenticationTestHelpers
{
    public static async Task AuthenticateTestUser1(this HttpClient client)
    {
        await AuthenticateUser(
            client: client,
            email: TestUserSeeder.TestUser1Email,
            password: TestUserSeeder.TestUser1Password);
    }

    public static async Task AuthenticateTestUser2(this HttpClient client)
    {
        await AuthenticateUser(
            client: client,
            email: TestUserSeeder.TestUser2Email,
            password: TestUserSeeder.TestUser2Password);
    }

    private static async Task AuthenticateUser(
        HttpClient client,
        string email,
        string password)
    {
        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await client.PostAsJsonAsync("/login", loginRequest);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Login failed. ReasonPhrase: {response.ReasonPhrase}");
        }

        var loginData = await response.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Authorization = null;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginData!.AccessToken);
    }

    private class LoginRequest
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    private class LoginResponse
    {
        [JsonPropertyName("accessToken")] public string AccessToken { get; set; }
    }
}