using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using Protocolli.IOT.Drone.ClientApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Protocolli.IOT.Drone.ClientApp.Protocols
{
	internal class Mqtt : IProtocol
	{
		private const string Topic = "gameofdrones/";
		private IMqttClient mqttClient;
		private string endpoint;

		public Mqtt(string endpoint)
		{
			this.endpoint = endpoint;

			Connect().GetAwaiter().GetResult();
		}

		private async Task<MqttClientConnectResult> Connect()
		{
			var factory = new MqttFactory();

			var options=new MqttClientOptionsBuilder()
				.WithTcpServer(this.endpoint)
				.Build();

			mqttClient = factory.CreateMqttClient();

			return await mqttClient.ConnectAsync(options,CancellationToken.None);
		}
		public async Task SendAsync(string data, string drone)
		{
			var message = new MqttApplicationMessageBuilder()
				.WithTopic(Topic + drone)
				.WithPayload(data)
				.WithExactlyOnceQoS()
				.Build();

			await mqttClient.PublishAsync(message,CancellationToken.None);
		}
	}
}
