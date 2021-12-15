using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;

namespace Protocolli.IoT.Drone.Infrastructure.Messaging
{
    public class MqttClientService
    {
        private readonly IManagedMqttClient _mqttClient;
        private readonly ILogger _logger;

        public MqttClientService(ILogger<MqttClientService> logger)
        {
            _logger = logger;
            _mqttClient = new MqttFactory().CreateManagedMqttClient();

            _mqttClient.UseConnectedHandler(e =>
            {
                _logger.LogInformation($"{DateTime.Now} Connected successfully with MQTT Broker.");
            });

            _mqttClient.UseDisconnectedHandler(e =>
            {
                _logger.LogInformation($"{DateTime.Now} Disconnected from MQTT Broker.");
            });
        }

        public void SetMessageReceivedHandler(Action<MqttApplicationMessageReceivedEventArgs> messageReceivedHandlerDelegate)
        {
            _mqttClient.UseApplicationMessageReceivedHandler(messageReceivedHandlerDelegate);
        }

        public async Task ConnectAsync(IMqttClientOptions clientOptions)
        {
            // Setup and start a managed MQTT client.
            var managedClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(clientOptions)
                .Build();

            // StartAsync returns immediately, as it starts a new thread using Task.Run, 
            // and so the calling thread needs to wait.
            await _mqttClient.StartAsync(managedClientOptions);
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