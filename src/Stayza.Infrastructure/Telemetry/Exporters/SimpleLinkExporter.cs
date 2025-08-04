using System.Diagnostics;
using OpenTelemetry;

namespace Stayza.Infrastructure.Telemetry.Exporters;

public class SimpleLinkExporter : BaseExporter<Activity>
{
    public override ExportResult Export(in Batch<Activity> batch)
    {
        foreach (var activity in batch)
        {
            if (string.IsNullOrEmpty(activity.ParentId))
            {
                Console.WriteLine($"http://localhost:18888/traces/detail/{activity.TraceId}");
            }
        }

        return ExportResult.Success;
    }
}