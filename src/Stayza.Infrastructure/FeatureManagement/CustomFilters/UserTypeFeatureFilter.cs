using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace Stayza.Infrastructure.FeatureManagement.CustomFilters;

[FilterAlias("UserType")]
public class UserTypeFeatureFilter : IFeatureFilter
{
    private readonly ILogger<UserTypeFeatureFilter> _logger;

    public UserTypeFeatureFilter(ILogger<UserTypeFeatureFilter> logger)
    {
        _logger = logger;
    }

    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    {
        _logger.LogInformation("Evaluating if user has feature enabled");
        
        return Task.FromResult(true);
    }
}