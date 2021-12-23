using CoAPNet;
using CoAPNet.Server;
using CoAPNet.Udp;
using Protocolli.IoT.Drone.ServerCoAP.Resources;

namespace Protocolli.IoT.Drone.ServerCoAP
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CoapServer _coapServer;
        private readonly CoapResourceHandler _resourceHandler;
        private readonly DroneStatusResource _droneStatusResource;

        public Worker(ILogger<Worker> logger, CoapServer coapServer, CoapResourceHandler resourceHandler, DroneStatusResource droneStatusResource)
        {
            _logger = logger;
            _coapServer = coapServer;
            _resourceHandler = resourceHandler;
            _droneStatusResource = droneStatusResource;

            _resourceHandler.Resources.Add(_droneStatusResource);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Listen to all ip address and subscribe to multicast requests.
            await _coapServer.BindTo(new CoapUdpEndPoint(Coap.Port) { JoinMulticast = true });

            // Start our server.
            await _coapServer.StartAsync(_resourceHandler, CancellationToken.None);

            _logger.LogInformation("Server Started!");
        }
    }
}