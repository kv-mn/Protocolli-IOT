using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Protocolli.IOT.Drone.ClientApp.Models;
using Protocolli.IOT.Drone.ClientApp.Protocols;
using Protocolli.IOT.Drone.ClientApp.Interfaces;
namespace Protocolli.IOT.Drone.Sender
{
	public class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;
		ClientApp.Interfaces.IProtocol sender = new Mqtt();

		public Worker(ILogger<Worker> logger)
		{
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using var connection = factory.CreateConnection();
			using var channel = connection.CreateModel();

			channel.QueueDeclare(queue: "drone-status",
								 durable: true,
								 exclusive: false,
								 autoDelete: false,
								 arguments: null);
			
			
				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += (model, ea) =>
				{

					var body = ea.Body.ToArray();
					var message = Encoding.UTF8.GetString(body);
					var dronestatus = JsonSerializer.Deserialize<DroneStatus>(message);

					_logger.LogInformation(message);
					sender.SendAsync(dronestatus);
				};
				channel.BasicConsume(queue: "drone-status",
									 autoAck: true,
									 consumer: consumer);

				
			

			Thread.Sleep(2000);
			Console.ReadLine();
		}
	}
}