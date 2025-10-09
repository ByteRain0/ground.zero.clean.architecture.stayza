using Yarp.ReverseProxy.LoadBalancing;
using Yarp.ReverseProxy.Model;

public class TenantBasedLoadBalancingPolicy : ILoadBalancingPolicy
{
    public string Name => "TenantBased";
    public DestinationState? PickDestination(HttpContext context, ClusterState cluster, IReadOnlyList<DestinationState> availableDestinations)
    {
        if (context.Request.Headers.TryGetValue("TenantId", out var tenantIdHeaderValue))
        {
            return availableDestinations.First(x => x.DestinationId == tenantIdHeaderValue);
        }

        return availableDestinations[^1];
        
    }
}