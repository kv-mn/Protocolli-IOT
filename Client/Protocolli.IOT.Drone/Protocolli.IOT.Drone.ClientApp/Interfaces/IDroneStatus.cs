using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocolli.IOT.Drone.ClientApp.Interfaces
{
    public interface IDroneStatus
    {
        public string SimulateDeviceStatus();
        public int GetDroneId();
    }
}
