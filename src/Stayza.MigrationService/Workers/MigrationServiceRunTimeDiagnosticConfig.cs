using System.Diagnostics;

namespace Stayza.MigrationService.Workers;

public static class MigrationServiceRunTimeDiagnosticConfig
{
    public const string ServiceName = "migration-service";

    public static string ServiceVersion = typeof(MigrationServiceRunTimeDiagnosticConfig).Assembly.GetName().Version?.ToString() ?? "unknown";

    public static ActivitySource Source = new(ServiceName);
}