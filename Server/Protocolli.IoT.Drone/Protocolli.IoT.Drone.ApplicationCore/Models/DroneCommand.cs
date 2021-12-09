using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocolli.IoT.Drone.ApplicationCore.Models
{
    public class DroneCommand
    {
        public int DroneId { get; set; }
        public string Command { get; set; }
        public bool Value { get; set; }
        public long TimeStamp { get; set; }
    }
}
