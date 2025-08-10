namespace Notifications.Web.Greetings;

public class RealHelloService : IHelloService
{
    public string GetHelloMessage()
    {
        return "Hello from real instance";
    }
}