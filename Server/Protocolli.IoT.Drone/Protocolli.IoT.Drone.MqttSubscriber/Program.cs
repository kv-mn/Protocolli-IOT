using Protocolli.IoT.Drone.ApplicationCore.Interfaces.Data;
using Protocolli.IoT.Drone.ApplicationCore.Interfaces.Services;
using Protocolli.IoT.Drone.ApplicationCore.Services;
using Protocolli.IoT.Drone.Infrastructure.Data;
using Protocolli.IoT.Drone.Infrastructure.Messaging;
using Protocolli.IoT.Drone.MqttSubscriber;
using System.Reflection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config => config.AddUserSecrets(Assembly.GetExecutingAssembly()))
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IDroneStatusService, DroneStatusService>();
        services.AddSingleton<IDroneStatusRepository, DroneStatusRepository>();
        services.AddSingleton<MqttClientService>();
    })
    .Build();

await host.RunAsync();
