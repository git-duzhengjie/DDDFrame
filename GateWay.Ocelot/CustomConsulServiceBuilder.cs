using Consul;
using Ocelot.Logging;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Consul.Interfaces;
using Ocelot.Values;

internal class CustomConsulServiceBuilder : DefaultConsulServiceBuilder
{
    public CustomConsulServiceBuilder(IHttpContextAccessor contextAccessor, IConsulClientFactory clientFactory, IOcelotLoggerFactory loggerFactory) : base(contextAccessor, clientFactory, loggerFactory)
    {
    }

    protected override string GetDownstreamHost(ServiceEntry entry, Node node)
    {
         return entry.Service.Address;
    }
}