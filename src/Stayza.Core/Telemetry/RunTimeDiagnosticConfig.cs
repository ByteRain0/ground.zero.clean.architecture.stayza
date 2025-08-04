using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Stayza.Core.Telemetry;

public static class RunTimeDiagnosticConfig
{
    public const string ServiceName = "dotnet-api";

    public static string ServiceVersion = "1.0";
    
    public static ActivitySource Source = new(ServiceName);
    
    public static Meter Meter = new(ServiceName, ServiceVersion);
    
    public static Counter<int> ChampionshipParticipationRequests =
        Meter.CreateCounter<int>("participation_requests", "Number of partification requests");
}