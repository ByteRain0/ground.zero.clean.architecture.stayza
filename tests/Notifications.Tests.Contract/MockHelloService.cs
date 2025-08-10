using Notifications.Web.Greetings;

namespace Notifications.Tests.Contract;

public class MockHelloService : IHelloService
{
    public string GetHelloMessage()
    {
        return "Hello from Mock";
    }
};