using CoAP.Server;
using Protocolli.IoT.Drone.ServerCoAP.Resources;

namespace Protocolli.IoT.Drone.ServerCoAP
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CoapServer _coapServer;
        private readonly DroneStatusResource _droneStatusResource;

        public Worker(ILogger<Worker> logger, CoapServer coapServer, DroneStatusResource droneStatusResource)
        {
            _logger = logger;
            _coapServer = coapServer;
            _droneStatusResource = droneStatusResource;

            _coapServer.Add(_droneStatusResource);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _coapServer.Start();
            _logger.LogInformation("Server Started!");
        }
    }
}