using CoAP;
using CoAP.Server.Resources;
using Protocolli.IoT.Drone.ApplicationCore.Interfaces.Services;
using Protocolli.IoT.Drone.ApplicationCore.Models;
using System.Text.Json;

namespace Protocolli.IoT.Drone.ServerCoAP.Resources
{
    public class DroneStatusResource : Resource
    {
        private readonly ILogger<DroneStatusResource> _logger;
        private readonly IDroneStatusService _droneStatusService;

        public DroneStatusResource(ILogger<DroneStatusResource> logger, IDroneStatusService droneStatusService) : base("DroneStatus")
        {
            _logger = logger;
            _droneStatusService = droneStatusService;
        }

        protected override void DoPost(CoapExchange exchange)
        {
            _logger.LogInformation($"Got request: {exchange.Request}");

            var payload = exchange.Request.PayloadString;
            DroneStatus droneStatus;
            Response response;

            try
            {
                droneStatus = JsonSerializer.Deserialize<DroneStatus>(payload);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex.Message);

                // 4.00 Bad request
                response = new Response(StatusCode.BadRequest);

                exchange.Respond(response);
                return;
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex.Message);

                // 4.00 Bad request
                response = new Response(StatusCode.BadRequest);

                exchange.Respond(response);
                return;
            }

            try
            {
                _droneStatusService.InsertDroneStatus(droneStatus);
            }
            catch (InfluxDB.Client.Core.Exceptions.HttpException ex)
            {
                _logger.LogError(ex.Message);

                // 5.00 Internal Server Error
                response = new Response(StatusCode.InternalServerError);
                exchange.Respond(response);
                return;
            }


            // 2.04 Changed
            response = new Response(StatusCode.Changed);
            exchange.Respond(response);
        }
    }
}
