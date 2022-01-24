using Microsoft.AspNetCore.Mvc;
using Protocolli.IoT.Drone.ApplicationCore.Interfaces.Services;
using Protocolli.IoT.Drone.ApplicationCore.Models;

namespace Protocolli.IoT.Drone.ServerApp.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class DroneStatusController : ControllerBase
    {
        private readonly IDroneStatusService _droneStatusService;
        private readonly ILogger<DroneStatusController> _logger;

        public DroneStatusController(IDroneStatusService droneStatusService, ILogger<DroneStatusController> logger)
        {
            _droneStatusService = droneStatusService;
            _logger = logger;
        }

        // POST DroneStatus/{droneId}
        [HttpPost("{droneId}")]
        public IActionResult Insert(int droneId, DroneStatus droneStatus)
        {
            _logger.LogInformation($"Got request at: /v1/DroneStatus/{droneId}");

            if (ModelState.IsValid)
            {
                try
                {
                    _droneStatusService.InsertDroneStatus(droneStatus);

                }
                catch (InfluxDB.Client.Core.Exceptions.HttpException ex)
                {
                    _logger.LogError(ex.Message);
                    return Problem(statusCode: 500);
                }

                _logger.LogInformation("Successfully saved to DB");
                return NoContent();
            }

            _logger.LogError("Invalid payload");
            return BadRequest(ModelState);
        }
    }
}
