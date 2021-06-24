using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Configuration;
using AnomalyService.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AnomalyService.Helpers
{
    public class RabbitMQHelper
    {
        private string Host;
        public RabbitMQHelper()
        {
           this.Host= Startup.Configuration.GetConnectionString("HostName");
        }
        public void SendMessage(string msg,string routekey)
        {
            var factory = new ConnectionFactory() { HostName = Host };
            
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: routekey,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                
                var body = Encoding.UTF8.GetBytes(msg);

                channel.BasicPublish(exchange: "",
                                     routingKey: routekey,
                                     basicProperties: null,
                                     body:body);
            }
        }
    }
}
