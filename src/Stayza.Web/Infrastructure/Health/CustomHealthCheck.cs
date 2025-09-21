using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Stayza.Web.Infrastructure.Health;

public class CustomHealthCheck : IHealthCheck
{
    private readonly ILogger<CustomHealthCheck> _logger;

    public CustomHealthCheck(ILogger<CustomHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = new CancellationToken())
    {
        _logger.LogInformation("Health checking");
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}