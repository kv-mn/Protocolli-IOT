using MQTTnet.Client.Receiving;
using Protocolli.IoT.Drone.ApplicationCore.Models;
using Protocolli.IoT.Drone.Infrastructure.Messaging;
using System.Text.Json;

namespace Protocolli.IoT.Drone.MqttPublisher
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly MqttClientService _mqttClient;

        public Worker(ILogger<Worker> logger, MqttClientService mqttClient)
        {
            _logger = logger;
            _mqttClient = mqttClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTime.Now);
            await _mqttClient.ConnectAsync();

            const int droneCount = 3;

            while (true)
            {
                for (int i = 0; i < droneCount; i++)
                {
                    var droneCommand = DroneCommand.RandomDroneCommand(i);
                    var payload = JsonSerializer.Serialize(droneCommand);
                    var topic = $"gameofdrones/{droneCommand.DroneId}/commands";
                    var qos = 2;

                    await _mqttClient.PublishAsync(topic, payload, qos, true);

                    _logger.LogInformation($"{DateTime.Now} | Publish | Topic: {topic} | QoS: {qos}");
                }

                Thread.Sleep(2000);
            }
        }
    }
}