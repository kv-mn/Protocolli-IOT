using MQTTnet;
using MQTTnet.Client.Receiving;
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
        private readonly MqttClient _mqttClient;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IDroneStatusService droneStatusService)
        {
            _logger = logger;
            _droneStatusService = droneStatusService;

            var mqttDelegate = new MqttApplicationMessageReceivedHandlerDelegate(OnSubscriberMessageReceived);

            _mqttClient = new MqttClient(configuration, mqttDelegate);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await _mqttClient.ConnectAsync();
            await _mqttClient.SubscribeAsync("gameofdrones/+/status");
        }

        private void OnSubscriberMessageReceived(MqttApplicationMessageReceivedEventArgs x)
        {
            var item = $"{DateTimeOffset.Now} | Topic: {x.ApplicationMessage.Topic} | QoS: {x.ApplicationMessage.QualityOfServiceLevel}";

            Console.WriteLine(item);    

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