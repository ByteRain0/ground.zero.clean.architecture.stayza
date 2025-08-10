using Notifications.Web.Infrastructure.Startup;

namespace Notifications.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build<Startup>();
        app.Run();
    }
}