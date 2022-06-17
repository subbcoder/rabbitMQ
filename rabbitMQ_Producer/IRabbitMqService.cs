using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace rabbitMQ
{
    public interface IRabbitMqService
	{
		void SendMessage(object obj);
		void SendMessage(string message);
	}

	public class RabbitMqService : IRabbitMqService
	{
		public void SendMessage(object obj)
		{
			var message = JsonSerializer.Serialize(obj);
			SendMessage(message);
		}

		public void SendMessage(string message)
		{
			// Не забудьте вынести значения "localhost" и "MyQueue"
			// в файл конфигурации
			var factory = new ConnectionFactory() {
				UserName = "guest", 
				Password = "guest", 
				HostName = "10.10.11.18", 
				Port = 5672, 
				VirtualHost = "/"
			};
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
                channel.ExchangeDeclare(
                    exchange: "my_exchange",
                    type: "direct",
                    durable: false,
                    autoDelete: false,
                    arguments: null
                );

                channel.QueueDeclare(queue: "MyQueue",
							   durable: false,
							   exclusive: false,
							   autoDelete: false,
							   arguments: null);

				var body = Encoding.UTF8.GetBytes(message);

				channel.BasicPublish(exchange: "",
							   routingKey: "MyQueue",
							   basicProperties: null,
							   body: body);
			}
		}
	}
}
