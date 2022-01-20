using Protocolli.IOT.Drone.ClientApp.Interfaces;
using Protocolli.IOT.Drone.ClientApp.Models;
using Protocolli.IOT.Drone.ClientApp.Protocols;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
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
			//Protocolli.IOT.Drone.ClientApp.Interfaces.IProtocol sender = new Mqtt();

			Console.WriteLine("Quanti droni vuoi simulare?");
			int dronesNumber = int.Parse(Console.ReadLine());

			for (int i = 0; i < dronesNumber; i++)
			{
				devices.Add(new DroneStatus() { DroneId = i });
			}

			var factory = new ConnectionFactory() { HostName = "localhost" };
			using var connection = factory.CreateConnection();
			using var channel = connection.CreateModel();

			channel.QueueDeclare(queue: "drone-status",
									durable: true,
									exclusive: false,
									autoDelete: false,
									arguments: null);
			while (true)
			{
				for (int i = 0; i < devices.Count; i++)
				{
					string message = devices[i].SimulateDeviceStatus();
					var body = Encoding.UTF8.GetBytes(message);

					channel.BasicPublish(exchange: "protocolli-iot",
									 routingKey: "queue",
									 basicProperties: null,
									 body: body);
					Console.WriteLine(" [x] Sent {0}", message);

				}
				Thread.Sleep(2000);
			}


			/*while (true)
				 {
					 for (int i = 0; i < devices.Count; i++)
					 {
						 await sender.SendAsync(devices[i]);
					 }

					 Thread.Sleep(2000);

				 }*/


		}
	}
}
