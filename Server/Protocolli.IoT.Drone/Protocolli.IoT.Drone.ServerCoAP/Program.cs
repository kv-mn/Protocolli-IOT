using CoAP.Server;
using Protocolli.IoT.Drone.ApplicationCore.Interfaces.Data;
using Protocolli.IoT.Drone.ApplicationCore.Interfaces.Services;
using Protocolli.IoT.Drone.ApplicationCore.Services;
using Protocolli.IoT.Drone.Infrastructure.Data;
using Protocolli.IoT.Drone.ServerCoAP;
using Protocolli.IoT.Drone.ServerCoAP.Resources;
using System.Reflection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config => config.AddUserSecrets(Assembly.GetExecutingAssembly()))
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<CoapServer>();
        services.AddSingleton<DroneStatusResource>();
        services.AddSingleton<DroneCommandResource>();
        services.AddSingleton<IDroneStatusService, DroneStatusService>();
        services.AddSingleton<IDroneStatusRepository, DroneStatusRepository>();
    })
    .Build();

await host.RunAsync();
