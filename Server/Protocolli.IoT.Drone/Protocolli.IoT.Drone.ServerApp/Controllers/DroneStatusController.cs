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

        public DroneStatusController(IDroneStatusService droneStatusService)
        {
            _droneStatusService = droneStatusService;
        }

        // POST batteries
        [HttpPost]
        public IActionResult Insert(DroneStatus droneStatus)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _droneStatusService.InsertDroneStatus(droneStatus);
                }
                catch (InfluxDB.Client.Core.Exceptions.HttpException ex)
                {

                    return Problem(statusCode: 500);
                }

                return NoContent();
            }

            return BadRequest(ModelState);
        }
    }
}
