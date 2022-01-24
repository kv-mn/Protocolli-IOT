using Protocolli.IOT.Drone.ClientApp.Interfaces;
using Protocolli.IOT.Drone.ClientApp.Models;
using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Protocolli.IOT.Drone.ClientApp.Protocols
{
    internal class Http : IProtocol
    {
        //set the URL of the API
        private readonly string _url  = ConfigurationManager.AppSettings["httpAPI"];
        private readonly HttpClient _httpClient = new();

        public async Task SendAsync(string data)
        {
            try
            {
                var droneStatus = JsonSerializer.Deserialize<DroneStatus>(data);
                var url = $"{_url}/{droneStatus.DroneId}";
                var response = await _httpClient.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json"));
                Console.WriteLine($"{url} responded with status code: {response.StatusCode}");
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}
