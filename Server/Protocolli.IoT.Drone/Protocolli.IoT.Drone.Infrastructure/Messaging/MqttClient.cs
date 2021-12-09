using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;

namespace Protocolli.IoT.Drone.Infrastructure.Messaging
{
    public class MqttClient
    {
        private readonly string _clientId;
        private readonly string _brokerUrl;

        private readonly IManagedMqttClient _mqttClient;

        public MqttClient(IConfiguration configuration, MqttApplicationMessageReceivedHandlerDelegate OnMessageReceived)
        {
            _clientId = configuration.GetSection("MQTT")["clientId"];
            _brokerUrl = configuration.GetSection("MQTT")["brokerUrl"];

            _mqttClient = new MqttFactory().CreateManagedMqttClient();

            _mqttClient.ApplicationMessageReceivedHandler = OnMessageReceived;
            SetHandlers();
        }

        private void SetHandlers()
        {
            _mqttClient.UseConnectedHandler(e =>
            {
                Console.WriteLine($"Connected successfully with MQTT Broker at {_brokerUrl}.");
            });

            _mqttClient.UseDisconnectedHandler(e =>
            {
                Console.WriteLine($"Disconnected from MQTT Broker at {_brokerUrl}.");
            });
        }

        public async Task ConnectAsync()
        {
            // Setup and start a managed MQTT client.
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(_clientId)
                    .WithTcpServer(_brokerUrl)
                    .Build())
                .Build();

            // StartAsync returns immediately, as it starts a new thread using Task.Run, 
            // and so the calling thread needs to wait.
            await _mqttClient.StartAsync(options);
        }

        public async Task PublishAsync(string topic, string payload, bool retainFlag = true, int qos = 0)
        {
            await _mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
              .WithTopic(topic)
              .WithPayload(payload)
              .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
              .WithRetainFlag(retainFlag)
              .Build());
        }

        public async Task SubscribeAsync(string topic, int qos = 0)
        {
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
              .WithTopic(topic)
              .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
              .Build());
        }
    }
}
