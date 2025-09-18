namespace Stayza.Infrastructure.ExternalConfigurations;

public class AppConfiguration
{
    public ApplicationInformation ApplicationInformation { get; set; }
    
    public string[] Configs { get; set; }

    public string Url { get; set; }
}

public class ApplicationInformation
{
    public string Name { get; set; }

    public int Version { get; set; }
}