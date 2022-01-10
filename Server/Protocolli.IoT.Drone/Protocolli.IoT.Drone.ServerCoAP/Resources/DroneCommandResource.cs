using CoAP;
using CoAP.Server.Resources;
using Protocolli.IoT.Drone.ApplicationCore.Models;
using System.Text.Json;
using System.Timers;

namespace Protocolli.IoT.Drone.ServerCoAP.Resources
{
    public class DroneCommandResource : Resource
    {
        private readonly System.Timers.Timer _timer;
        private readonly ILogger<DroneCommandResource> _logger;

        public DroneCommandResource(ILogger<DroneCommandResource> logger) : base("DroneCommand")
        {
            Observable = true;

            _timer = new System.Timers.Timer(5000);
            _timer.Elapsed += OnTimedEvent;
            _timer.Enabled = true;
            _logger = logger;
        }

        protected override void DoGet(CoapExchange exchange)
        {
            _logger.LogInformation($"Got request: {exchange.Request}");

            var command = DroneCommand.RandomDroneCommand(1);
            var payload = JsonSerializer.Serialize(command);
            var response = new Response(StatusCode.Content);
            response.PayloadString = payload;
            exchange.Respond(payload);
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Changed();
        }
    }
}
