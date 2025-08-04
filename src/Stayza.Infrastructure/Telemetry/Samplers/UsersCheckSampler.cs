using System.Collections.Immutable;
using Microsoft.AspNetCore.Http;
using OpenTelemetry.Trace;

namespace Stayza.Infrastructure.Telemetry.Samplers;

public class PremiumUsersCheckSampler<T> : Sampler where T : Sampler
{
    private static Random _random = new();
    private readonly UserTiers _keepPercentages;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly T _innerSampler;

    public PremiumUsersCheckSampler(
        UserTiers keepPercentages,
        IHttpContextAccessor contextAccessor,
        T innerSampler)
    {
        _keepPercentages = keepPercentages;
        _contextAccessor = contextAccessor;
        _innerSampler = innerSampler;
    }

    public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
    {
        var userTier = _contextAccessor.HttpContext?.Request.Headers["Tier"];

        if (userTier is null)
        {
            // You can chain samplers in this fashion like aspnet middleware
            // The use case would be that if a user tier is not defined do not sample ex: public pages of an app etc.
            // Gotchas:
            // 1. you would normally want to track some metrics there as well.
            // 2. You can simply return new SamplingResult(SamplingDecision.Drop) and be done with it.
            // 3. The part with innerSampler is just PoC.
            return _innerSampler.ShouldSample(samplingParameters);
        }

        var shouldSample = false;

        switch (userTier)
        {
            case UserTiers.BasicTierHeader:
                shouldSample = _random.Next(1, 100) < _keepPercentages.BasicTier;
                break;
            case UserTiers.StandardTierHeader:
                shouldSample = _random.Next(1, 100) < _keepPercentages.StandardTier;
                break;
            case UserTiers.PremiumTierHeader:
                shouldSample = _random.Next(1, 100) < _keepPercentages.PremiumTier;
                break;
            default:
                break;
        }

        if (shouldSample)
        {
            var samplingAttributes = ImmutableList.CreateBuilder<KeyValuePair<string, object>>();
            samplingAttributes.Add(new(UserTiers.UserTierTag, userTier));

            return new SamplingResult(SamplingDecision.RecordAndSample, samplingAttributes.ToImmutableList());
        }

        return new SamplingResult(SamplingDecision.Drop);
    }
}

public class UserTiers
{
    public static string UserTierTag = nameof(UserTiers);

    public const string BasicTierHeader = nameof(BasicTier);
    public int BasicTier { get; set; } = 1;


    public const string StandardTierHeader = nameof(StandardTier);
    public int StandardTier { get; set; } = 10;


    public const string PremiumTierHeader = nameof(PremiumTier);
    public int PremiumTier { get; set; } = 15;
}