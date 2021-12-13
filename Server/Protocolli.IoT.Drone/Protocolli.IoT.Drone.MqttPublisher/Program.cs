using Protocolli.IoT.Drone.Infrastructure.Messaging;
using Protocolli.IoT.Drone.MqttPublisher;
using System.Reflection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config => config.AddUserSecrets(Assembly.GetExecutingAssembly()))
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<MqttClient>();
    })
    .Build();

await host.RunAsync();
