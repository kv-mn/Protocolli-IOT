using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Protocolli.IOT.Drone.ClientApp.Models
{
	internal class DroneCommand : DroneStatus
	{
		private readonly IMqttClient _mqttClient;
		private readonly string _url = ConfigurationManager.AppSettings["brokerMQTT"];
		private readonly int _port = int.Parse(ConfigurationManager.AppSettings["portMQTT"]);
		private readonly string _topic = ConfigurationManager.AppSettings["topicMQTTcommands"];

		public DroneCommand()
		{
			var factory = new MqttFactory();
			_mqttClient=factory.CreateMqttClient();

			var options = new MqttClientOptionsBuilder()
				.WithTcpServer(_url,_port)
				.WithCleanSession(false)
				.Build();

			_mqttClient.ConnectAsync(options, CancellationToken.None);

			_mqttClient.UseDisconnectedHandler(async e =>
			{
				Console.WriteLine("Disconnected from Server");
				await Task.Delay(TimeSpan.FromSeconds(5));

				try
				{
					await _mqttClient.ConnectAsync(options, CancellationToken.None);
				}
				catch
				{
					Console.WriteLine("Reconnecting Failed");
				}
			});

			_mqttClient.UseConnectedHandler(async e =>
			{
				Console.WriteLine("Connected with Server");
				await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
					.WithTopicFilter($"gameofdrones/{DroneId}/commands")
					.Build());

				Console.WriteLine($"Subscribe to topic: gameofdrones/{DroneId}/commands");
			});
			_mqttClient.UseApplicationMessageReceivedHandler(e =>
			{
				string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
				Console.WriteLine($"Command Receive at Drone {payload}");


			});
		}

	}
}
