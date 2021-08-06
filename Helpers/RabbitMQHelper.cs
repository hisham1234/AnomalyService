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
using System.Reflection;

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
            this.Host = Startup.Configuration.GetSection("AZURESETTINGS").GetValue<string>("AZURE_CONTAINER_NAME");
            this.Host = Startup.Configuration.GetConnectionString("RABBITMQ_HOSTNAME");
            this.UserName = Startup.Configuration.GetConnectionString("RABBITMQ_USERNAME");
            this.Password = Startup.Configuration.GetConnectionString("RABBITMQ_PASSWORD");
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
            var logmsg = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + " |" + Assembly.GetCallingAssembly().GetName().Name + " |" + routekey + " Message Was Published ";

            Console.WriteLine(logmsg);
        }
    }
}
