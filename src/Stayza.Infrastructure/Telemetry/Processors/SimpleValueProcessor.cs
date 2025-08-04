using System.Diagnostics;
using OpenTelemetry;

namespace Stayza.Infrastructure.Telemetry.Processors;

public class SimpleValueProcessor : BaseProcessor<Activity>
{
    public override void OnStart(Activity data)
    {
        data.AddTag("static.value", "static value to add to the traces");
        data.AddTag("app.name", "stayza.web");
    }
}