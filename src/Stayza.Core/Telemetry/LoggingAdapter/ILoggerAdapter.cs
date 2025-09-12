namespace Stayza.Core.Telemetry.LoggingAdapter;

public interface ILoggerAdapter<T>
{
    void LogInformation(string messageTemplate, params object?[] args);
}