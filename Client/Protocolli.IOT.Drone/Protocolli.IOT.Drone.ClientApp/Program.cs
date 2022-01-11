using Protocolli.IOT.Drone.ClientApp.Interfaces;
using Protocolli.IOT.Drone.ClientApp.Models;
using Protocolli.IOT.Drone.ClientApp.Protocols;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Protocolli.IOT.Drone.ClientApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            List<IDroneStatus> devices = new();
            //IProtocol sender = new Http();
            IProtocol sender = new Coap();

            Console.WriteLine("Quanti droni vuoi simulare?");
            int dronesNumber = int.Parse(Console.ReadLine());

            for (int i = 0; i < dronesNumber; i++)
            {
                devices.Add(new DroneStatus() { DroneId = i });
            }

           

            while (true)
            {
                for (int i = 0; i < devices.Count; i++)
                {
                    await sender.SendAsync(devices[i].SimulateDeviceStatus());
                }

                Thread.Sleep(2000);

            }
            
        }
    }
}
