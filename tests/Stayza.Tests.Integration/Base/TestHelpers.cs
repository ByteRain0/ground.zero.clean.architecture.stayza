using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Stayza.Web.Infrastructure.Seed;

namespace Stayza.Tests.Integration.Base;

public static class TestHelpers
{
    /// <summary>
    /// Authenticates the test user.
    /// Sets the bearer token into the provided http client.
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<string> AuthenticateTestUser(this HttpClient client)
    {
        var loginRequest = new LoginRequest()
        {
            Email = TestUserSeeder.TestUserEmail,
            Password = TestUserSeeder.TestUserPassword
        };

        var response = await client.PostAsJsonAsync("/login", loginRequest);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Login failed. ReasonPhrase: {response.ReasonPhrase}");
        }

        var loginData = await response.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginData!.AccessToken);
        
        return loginData.AccessToken;
    }
    
    private class LoginRequest
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
    
    private class LoginResponse
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }
    }

}