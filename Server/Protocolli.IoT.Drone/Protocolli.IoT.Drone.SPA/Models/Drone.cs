using Protocolli.IoT.Drone.ApplicationCore.Models;

namespace Protocolli.IoT.Drone.SPA.Models
{
    public class Drone
    {
        public int DroneId { get; set; }
        public DroneStatus LastStatus { get; set; }
    }
}
