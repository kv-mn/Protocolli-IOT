using MQTTnet;
using MQTTnet.Client.Options;
using Protocolli.IoT.Drone.ApplicationCore.Interfaces.Services;
using Protocolli.IoT.Drone.ApplicationCore.Models;
using Protocolli.IoT.Drone.Infrastructure.Messaging;
using System.Text.Json;

namespace Protocolli.IoT.Drone.MqttSubscriberAdditional
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDroneStatusService _droneStatusService;
        private readonly MqttClientService _mqttClient;
        private readonly string _clientId;
        private readonly string _brokerUrl;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IDroneStatusService droneStatusService, MqttClientService mqttClient)
        {
            var mqttConfiguration = configuration.GetSection("MQTT");
            _clientId = mqttConfiguration["clientId"];
            _brokerUrl = mqttConfiguration["brokerUrl"];

            _logger = logger;
            _droneStatusService = droneStatusService;
            _mqttClient = mqttClient;

            _mqttClient.SetMessageReceivedHandler(MessageReceivedHandler);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var clientOptions = new MqttClientOptionsBuilder()
                .WithClientId(_clientId)
                .WithTcpServer(_brokerUrl)
                .WithCleanSession(true)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(5))
                .Build();

            _logger.LogInformation("Worker running at: {time}", DateTime.Now);
            await _mqttClient.ConnectAsync(clientOptions);
            await _mqttClient.SubscribeAsync("$share/subscribers/gameofdrones/+/status", 0);
        }

        private void MessageReceivedHandler(MqttApplicationMessageReceivedEventArgs x)
        {
            var item = $"{DateTime.Now} | Received | Topic: {x.ApplicationMessage.Topic} | QoS: {(int)x.ApplicationMessage.QualityOfServiceLevel}";

            _logger.LogInformation(item);

            var payload = x.ApplicationMessage.ConvertPayloadToString();

            try
            {
                var droneStatus = JsonSerializer.Deserialize<DroneStatus>(payload);
                _droneStatusService.InsertDroneStatus(droneStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process the message: {ex.Message}");
            }
        }
    }
}