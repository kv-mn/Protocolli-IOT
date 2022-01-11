using Protocolli.IOT.Drone.ClientApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoAP;

namespace Protocolli.IOT.Drone.ClientApp.Protocols
{
	internal class Coap :IProtocol
	{
        private readonly string _url = ConfigurationManager.AppSettings["coapServer"];

		public Task SendAsync(string data)
		{
            //create a new pos request
            Request _request = new(Method.POST);
            _request.URI = new Uri(_url);

            //handle reponses asynchronously with event handler
            _request.Respond += (o, e) =>
            {
                Console.WriteLine("Responded with status code: " + e.Message.StatusCode);
            };

            //set the request payload
            _request.SetPayload(data, MediaType.ApplicationJson);
            //send the request
            _request.Send();

            Console.WriteLine("Status sent");
            return Task.CompletedTask;
        }
	}
}
     