using CoAPNet;
using CoAPNet.Options;
using Protocolli.IoT.Drone.ApplicationCore.Interfaces.Services;
using Protocolli.IoT.Drone.ApplicationCore.Models;
using System.Text;
using System.Text.Json;

namespace Protocolli.IoT.Drone.ServerCoAP.Resources
{
    public class DroneStatusResource : CoapResource
    {
        private readonly ILogger<DroneStatusResource> _logger;
        private readonly IDroneStatusService _droneStatusService;
        public DroneStatusResource(ILogger<DroneStatusResource> logger, IDroneStatusService droneStatusService) : base("/status")
        {
            // ??
            //Metadata.InterfaceDescription.Add("read");
            //Metadata.ResourceTypes.Add("message");
            //Metadata.Title = "Hello World";

            _logger = logger;
            _droneStatusService = droneStatusService;
        }

        public override CoapMessage Post(CoapMessage request)
        {
            _logger.LogInformation($"Got request: {request}");
            var payload = Encoding.UTF8.GetString(request.Payload);
            DroneStatus droneStatus;

            try
            {
                droneStatus = JsonSerializer.Deserialize<DroneStatus>(payload);
            }
            catch (JsonException ex)
            {
                // Bad request
                return new CoapMessage
                {
                    Code = CoapMessageCode.BadRequest,
                    Options = { new ContentFormat(ContentFormatType.TextPlain) },
                    Payload = Encoding.UTF8.GetBytes("Incorrect drone status format.")
                };
            }

            // todo
            try
            {
                _droneStatusService.InsertDroneStatus(droneStatus);
            }
            catch (Exception ex)
            {
                // Internal Error
                return new CoapMessage
                {
                    Code = CoapMessageCode.Post,
                    Options = { new ContentFormat(ContentFormatType.TextPlain) },
                    Payload = Encoding.UTF8.GetBytes("Internal error.")
                };
            }
            
            return new CoapMessage
            {
                Code = CoapMessageCode.Created,
                Options = { new ContentFormat(ContentFormatType.TextPlain) },
                Payload = Encoding.UTF8.GetBytes("Drone status successfully saved.")
            };
        }
    }
}
