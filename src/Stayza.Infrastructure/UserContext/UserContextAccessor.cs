using Stayza.Core.Context;

namespace Stayza.Infrastructure.UserContext;

public class UserContextAccessor : IUserContext
{
    /// <summary>
    /// A mock service for now.
    /// </summary>
    /// <returns></returns>
    public Guid CurrentUserId()
    {
        return Guid.Parse("c80b7ea7-aef9-4e17-8436-7a5c94934c55");
    }
}