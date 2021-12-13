using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NihFix.Ocelot.MixedRouting.Example.ConsulAgent
{
    public class ConsulHostedService : IHostedService
    {
        private CancellationTokenSource _cts;
        private readonly IConsulClient _consulClient;
        private readonly IOptions<ConsulConfig> _consulConfig;
        private readonly ILogger<ConsulHostedService> _logger;
        private readonly IServer _server;
        private string _registrationId;

        public ConsulHostedService(IConsulClient consulClient, IOptions<ConsulConfig> consulConfig,
            ILogger<ConsulHostedService> logger, IServer server)
        {
            _server = server;
            _logger = logger;
            _consulConfig = consulConfig;
            _consulClient = consulClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var features = _server.Features;
            var addresses = features.Get<IServerAddressesFeature>();
            var address = addresses.Addresses.First();
            
            var uri = new Uri(address);
            _registrationId = $"{_consulConfig.Value.ServiceName}-{uri.Port}";

            var registration = new AgentServiceRegistration()
            {
                ID = _registrationId,
                Name = _consulConfig.Value.ServiceName,
                Address = _consulConfig.Value.ServiceName,
                Port = uri.Port,
            };

            _logger.LogInformation("Registering in Consul");
            await _consulClient.Agent.ServiceDeregister(registration.ID, _cts.Token);
            await _consulClient.Agent.ServiceRegister(registration, _cts.Token);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            _logger.LogInformation("Deregistering from Consul");
            try
            {
                await _consulClient.Agent.ServiceDeregister(_registrationId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Deregisteration failed");
            }
        }
    }
}