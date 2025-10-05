using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Microsoft.Playwright;

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
        .Build();

    public const string WebApiUrl = "https://localhost:5211";

    private IPlaywright _playwright;

    public IBrowserContext _browser;

    private const string DatabaseConnectionString = "";


    public async Task InitializeAsync()
    {
        _dockerComposeServices.Start();
    }
    
    
    public async Task DisposeAsync()
    {
        _dockerComposeServices.Dispose();
    }

}