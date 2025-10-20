using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using Npgsql;
using Stayza.Infrastructure.Persistence;

namespace Stayza.Tests.EndToEnd;

public class TestBase : IAsyncLifetime
{
    private static readonly string ServicesDockerComposeFilePath = 
        Path.Combine(
            Directory.GetCurrentDirectory(), 
            "../../../../../docker-compose-e2e-tests.yaml");
    
    private static readonly string ObservabilityDockerComposeFilePath = 
        Path.Combine(
            Directory.GetCurrentDirectory(), 
            "../../../../../docker-compose-local-observability.yaml");

    private readonly ICompositeService _dockerComposeServices = new Builder()
        .UseContainer()
        .UseCompose()
        .FromFile(ServicesDockerComposeFilePath)
        .FromFile(ObservabilityDockerComposeFilePath)
        .RemoveOrphans()
        .WaitForHttp("web-app", WebAppUrl)
        .Build();
    
    public const string WebAppUrl = "http://localhost:6210";
    
    public const string WebApiUrl = "http://localhost:5210";
    
    private IPlaywright _playwright;

    public IBrowserContext _browser { get; private set; }

    private const string DatabaseConnectionString = "Host=localhost;Port=5432;Database=mydatabase;Username=myuser;Password=mypassword;";

    public readonly DbContextOptionsBuilder<ApplicationDbContext> DatabaseOptions = 
        new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(new NpgsqlConnection(DatabaseConnectionString));

    public HttpClient HttpClient;

    public async Task InitializeAsync()
    {
        _dockerComposeServices.Start(); // Run the docker compose file to spin up the containers
        
        _playwright = await Playwright.CreateAsync(); // Create an instance of Playwright
        IBrowser browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
        {
            SlowMo = 1000, // Slows down Playwright operations by the specified amount of milliseconds. Useful so that you can see what is going on.
            Headless = true // By default the browser will be headless -- we can't see the window or what's going on, to prevent that we set it to false.
        });

        // Creating a new instance of Browser will allow us to run it in isolation preventing issues related to data sharing like cookies, preferences etc.
        _browser = await browser.NewContextAsync(new BrowserNewContextOptions()
        {
            IgnoreHTTPSErrors = true 
        });

        HttpClient = new HttpClient();
        HttpClient.BaseAddress = new Uri(WebApiUrl);
        
    }
    
    
    public async Task DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
        if(_dockerComposeServices != null)
        {
            Console.WriteLine("Container logs");
            foreach(var container in _dockerComposeServices.Containers)
            {
                Console.WriteLine($"Logs for: {container.Name}");
                var logs = container.Logs();
                Console.WriteLine(logs);
            }
        }
        _dockerComposeServices.Dispose();
    }

}