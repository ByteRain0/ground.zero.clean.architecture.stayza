using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Stayza.Web.Infrastructure.Seed;

namespace Stayza.Tests.Integration.Base;

public static class HttpClientExtensions
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
    
    public static void InjectTraceContext(this HttpClient client, Activity activity)
    {
        if (activity == null)
            throw new ArgumentNullException(nameof(activity));
        
        // Remove existing trace headers if any, to avoid duplicates
        client.DefaultRequestHeaders.Remove("traceparent");
        client.DefaultRequestHeaders.Remove("tracestate");
        // Add any other headers your propagator uses if needed

        OtelTestFramework.Propagator.Inject(
            new PropagationContext(activity.Context, Baggage.Current),
            client,
            (c, key, value) =>
            {
                if (!c.DefaultRequestHeaders.Contains(key))
                    c.DefaultRequestHeaders.Add(key, value);
            });
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