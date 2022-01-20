using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocolli.IOT.Drone.ClientApp.Interfaces
{
    public interface IProtocol
    {
        public Task SendAsync(IDroneStatus status);
    }
}
