using Yarp.ReverseProxy.Transforms;

public static class ReverseProxyBuilderExtensions
{
    private const string CorrelationIdHeaderKey = "x-correlation-id";
    
    public static IReverseProxyBuilder AddCorrelationId(this IReverseProxyBuilder proxyBuilder)
    {
        proxyBuilder.AddTransforms(transforms =>
        {
            transforms.AddRequestTransform(transform =>
            {
                if (Enumerable.Any<KeyValuePair<string, IEnumerable<string>>>(transform.ProxyRequest.Headers, 
                        x => x.Key == CorrelationIdHeaderKey))
                {
                    return ValueTask.CompletedTask;
                }
                
                var correlationId = Guid.NewGuid().ToString("N");
                transform.ProxyRequest.Headers.Add(CorrelationIdHeaderKey,correlationId);
                
                return ValueTask.CompletedTask;
            });
        });

        return proxyBuilder;
    }
}