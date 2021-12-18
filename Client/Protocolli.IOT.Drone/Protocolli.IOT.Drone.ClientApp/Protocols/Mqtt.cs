using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using Protocolli.IOT.Drone.ClientApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Protocolli.IOT.Drone.ClientApp.Protocols
{
	internal class Mqtt : IProtocol
	{
		private readonly IMqttClient _mqttClient;
		private readonly string _url = ConfigurationManager.AppSettings["brokerMQTT"];
		private readonly int _port = int.Parse(ConfigurationManager.AppSettings["portMQTT"]);
		private readonly string _topic = ConfigurationManager.AppSettings["topicMQTT"];

		public Mqtt()
		{
			var factory = new MqttFactory();
			_mqttClient = factory.CreateMqttClient();

			var options = new MqttClientOptionsBuilder()
				.WithTcpServer(_url, _port)
				.WithCleanSession(true)
				.Build();

			_mqttClient.ConnectAsync(options, CancellationToken.None);

			_mqttClient.UseDisconnectedHandler(async e =>
			{
				Console.WriteLine("Disconnected from Server ");
				await Task.Delay(TimeSpan.FromSeconds(5));

				try
				{
					await _mqttClient.ConnectAsync(options, CancellationToken.None);
				}
				catch
				{
					Console.WriteLine("Reconnection Failed");
				}
			});



		}

		public async Task SendAsync(IDroneStatus status)
		{
			int id = status.GetDroneId();
			string data = status.SimulateDeviceStatus();

			var message = new MqttApplicationMessageBuilder()
				.WithTopic($"{_topic}{id}")
				.WithPayload(data)
				.WithRetainFlag(true)
				.WithAtMostOnceQoS()
				.Build();

			try
			{
				await _mqttClient.PublishAsync(message, CancellationToken.None);
				Console.WriteLine($"Published Drone Status to topic : gameofdrones/{id}/status");
			}
			catch (MQTTnet.Exceptions.MqttCommunicationException ex)
			{
				Console.WriteLine(ex.Message);
			}
			
		}
	}
}
