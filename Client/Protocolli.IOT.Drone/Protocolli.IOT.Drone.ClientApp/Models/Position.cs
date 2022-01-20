using Protocolli.IOT.Drone.ClientApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Protocolli.IOT.Drone.ClientApp.Models
{
    public class Position
    {
        private readonly Random _random = new();
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Position SimulatePosition()
        {

            X = _random.NextDouble() * 180;
            Y = _random.NextDouble() * 90;
            Z = _random.NextDouble() * 4000;

            return this;

        }
    }
}
