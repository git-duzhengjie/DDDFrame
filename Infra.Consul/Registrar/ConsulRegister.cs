using Consul;
using Infra.Consul.Configuration;
using Microsoft.Extensions.Options;

namespace Infra.Consul.Registrar
{
    public class ConsulRegister(IOptionsMonitor<ConsulConfig> consulConfig) : IConsulRegister
    {
        private readonly ConsulConfig consulConfig = consulConfig.CurrentValue;

        public async Task RegisterAsync()
        {
            var address = new Uri(consulConfig.Address);
            var client = new ConsulClient(options =>
            {
                options.Address = address;
            });
            var registration = new AgentServiceRegistration
            {
                ID= $"{consulConfig.ServiceName}.{DateTime.Now.Ticks}",
                Name=consulConfig.ServiceName,
                Meta=new Dictionary<string, string>() { ["Scheme"] =address.Scheme},
                Address=consulConfig.IP,
                Port=consulConfig.Port,
                Tags=consulConfig.ServerTags,
                Check=new AgentServiceCheck
                {
                    DeregisterCriticalServiceAfter=TimeSpan.FromSeconds(5),
                    Interval=TimeSpan.FromSeconds(10),
                    HTTP=$"http://{consulConfig.IP}:{consulConfig.Port}{consulConfig.HealthCheckUrl}",
                    Timeout=TimeSpan.FromSeconds(consulConfig.Timeout),
                },
                
            };
            await client.Agent.ServiceRegister(registration);
        }
    }
}
