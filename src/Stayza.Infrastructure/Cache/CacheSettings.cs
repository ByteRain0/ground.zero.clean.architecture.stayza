namespace Stayza.Infrastructure.Cache;

public class CacheSettings
{
    public bool Enabled { get; set; }

    public string ConnectionString { get; set; }

    public string IntanceName { get; set; } = "Stayza";
}