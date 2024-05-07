using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;

namespace MotorcycleRental.RabbitService
{
    public interface IRabbitBusService
    {
        void Publish (object data, string routingKey);
    }

    public class RabbitMQService : IRabbitBusService
    {
        private IConnection _connection;
        private readonly IModel _channel;
        private const string _exchange = "Mottu";
        public RabbitMQService()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            _connection = connectionFactory.CreateConnection("Mottu_Services");
            _channel = _connection.CreateModel();
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }

        public void Publish(object data, string routingKey)
        {
            var queue = _connection.CreateModel();

            queue.QueueDeclare(queue: routingKey, 
                                durable: false, 
                                exclusive: false, 
                                autoDelete: false, 
                                arguments: null);

            var payload = JsonConvert.SerializeObject(data);
            var bytearray = Encoding.UTF8.GetBytes(payload);
            _channel.BasicPublish(_exchange, routingKey, null, bytearray);
        }
    }
}