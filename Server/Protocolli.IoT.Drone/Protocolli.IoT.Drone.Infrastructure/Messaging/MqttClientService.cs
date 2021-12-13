using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;

namespace Protocolli.IoT.Drone.Infrastructure.Messaging
{
    public class MqttClientService
    {
        private readonly string _clientId;
        private readonly string _brokerUrl;
        private readonly IManagedMqttClient _mqttClient;
        private readonly ILogger _logger;

        public MqttClientService(IConfiguration configuration, ILogger<MqttClientService> logger)
        {
            var mqttConfiguration = configuration.GetSection("MQTT");
            _clientId = mqttConfiguration["clientId"];
            _brokerUrl = mqttConfiguration["brokerUrl"];

            _logger = logger;
            _mqttClient = new MqttFactory().CreateManagedMqttClient();

            _mqttClient.UseConnectedHandler(e =>
            {
                _logger.LogInformation($"{DateTime.Now} Connected successfully with MQTT Broker at {_brokerUrl}.");
            });

            _mqttClient.UseDisconnectedHandler(e =>
            {
                _logger.LogInformation($"{DateTime.Now} Disconnected from MQTT Broker at {_brokerUrl}.");
            }); 
        }

        public void SetMessageReceivedHandler(Action<MqttApplicationMessageReceivedEventArgs> messageReceivedHandlerDelegate)
        {
            _mqttClient.UseApplicationMessageReceivedHandler(messageReceivedHandlerDelegate);
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

        public async Task PublishAsync(string topic, string payload, int qos, bool retainFlag = false)
        {
            await _mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
              .WithTopic(topic)
              .WithPayload(payload)
              .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
              .WithRetainFlag(retainFlag)
              .Build());
        }

        public async Task SubscribeAsync(string topic, int qos)
        {
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
              .WithTopic(topic)
              .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
              .Build());
        }
    }
}