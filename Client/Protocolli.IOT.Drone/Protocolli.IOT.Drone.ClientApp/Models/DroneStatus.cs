using Protocolli.IOT.Drone.ClientApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Protocolli.IOT.Drone.ClientApp.Models
{
    public class DroneStatus : IDroneStatus
    {
        private readonly Random _random = new();
        private readonly Position _position = new();

        public int DroneId { get; set; }
        public Position Position { get; set; }
        public float Velocity { get; set; }
        public float Battery { get; set; }
        public long Timestamp { get; set; }

        public string SimulateDeviceStatus()
        {
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

            Velocity = (float)_random.NextDouble() * 200;
            Battery = _random.Next(0, 100);
            Position = _position.SimulatePosition();

            return JsonSerializer.Serialize(this);
        }

        public int GetDroneId()
        {
            return DroneId;
        }
    }
}
