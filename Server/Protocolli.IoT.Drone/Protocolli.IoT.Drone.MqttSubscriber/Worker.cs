using MQTTnet;
using Protocolli.IoT.Drone.ApplicationCore.Interfaces.Services;
using Protocolli.IoT.Drone.ApplicationCore.Models;
using Protocolli.IoT.Drone.Infrastructure.Messaging;
using System.Text.Json;

namespace Protocolli.IoT.Drone.MqttSubscriber
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDroneStatusService _droneStatusService;
        private readonly MqttClientService _mqttClient;

        public Worker(ILogger<Worker> logger, IDroneStatusService droneStatusService, MqttClientService mqttClient)
        {
            _logger = logger;
            _droneStatusService = droneStatusService;
            _mqttClient = mqttClient;

            _mqttClient.SetMessageReceivedHandler(OnSubscriberMessageReceived);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTime.Now);
            await _mqttClient.ConnectAsync();
            await _mqttClient.SubscribeAsync("gameofdrones/+/status", 0);
        }

        private void OnSubscriberMessageReceived(MqttApplicationMessageReceivedEventArgs x)
        {
            var item = $"{DateTime.Now} | Received | Topic: {x.ApplicationMessage.Topic} | QoS: {(int)x.ApplicationMessage.QualityOfServiceLevel}";

            _logger.LogInformation(item);    

            var payload = x.ApplicationMessage.ConvertPayloadToString();

            try
            {
                var droneStatus = JsonSerializer.Deserialize<DroneStatus>(payload);
                _droneStatusService.InsertDroneStatus(droneStatus);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}