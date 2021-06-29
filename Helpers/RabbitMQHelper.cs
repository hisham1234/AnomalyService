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
        private string UserName;
        private string Password;
        private string Exchange;
        public RabbitMQHelper()
        {
            this.Host = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME");
            this.Host = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME");
            this.Host = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");
            this.Exchange = Startup.Configuration.GetConnectionString("RABBITMQ_EXCHANGE_NAME");
        }
        public void SendMessage(string msg,string routekey)
        {
            var factory = new ConnectionFactory() { HostName = Host,Password=this.Password,UserName=this.UserName  };
            
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                               
                var body = Encoding.UTF8.GetBytes(msg);

                channel.BasicPublish(exchange: this.Exchange,
                                     routingKey: routekey,
                                    
                                     body:body);
            }
        }
    }
}
