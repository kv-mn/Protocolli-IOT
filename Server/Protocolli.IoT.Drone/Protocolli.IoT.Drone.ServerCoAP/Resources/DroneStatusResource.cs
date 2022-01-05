﻿using CoAPNet;
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
        public DroneStatusResource(ILogger<DroneStatusResource> logger, IDroneStatusService droneStatusService) : base("/DroneStatus")
        {
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
                _logger.LogError(ex.Message);

                // Bad request
                return new CoapMessage
                {
                    Code = CoapMessageCode.BadRequest,
                    Options = { new ContentFormat(ContentFormatType.TextPlain) },
                    Payload = Encoding.UTF8.GetBytes("Invalid payload")
                };
            }

            try
            {
                _droneStatusService.InsertDroneStatus(droneStatus);
            }
            catch (InfluxDB.Client.Core.Exceptions.HttpException ex)
            {
                _logger.LogError(ex.Message);

                // Internal Server Error
                return new CoapMessage
                {
                    Code = CoapMessageCode.InternalServerError,
                    Options = { new ContentFormat(ContentFormatType.TextPlain) },
                    Payload = Encoding.UTF8.GetBytes("Internal error")
                };
            }

            return new CoapMessage
            {
                Code = CoapMessageCode.Created,
                Options = { new ContentFormat(ContentFormatType.TextPlain) },
                Payload = Encoding.UTF8.GetBytes("Drone status successfully saved")
            };
        }
    }
}
