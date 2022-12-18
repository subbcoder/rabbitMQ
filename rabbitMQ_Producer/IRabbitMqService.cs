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
				UserName = "user", 
				Password = "1234", 
				HostName = "10.10.11.18", 
				Port = 5672, 
				VirtualHost = "/"
			};
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
                channel.ExchangeDeclare(
                    exchange: "my_exchange",
                    type: "direct", //  topic fanout 
                    durable: false,
                    autoDelete: false,
                    arguments: null
                );

				// Создаем две очереди
                channel.QueueDeclare(queue: "MyQueue",
							   durable: false,
							   exclusive: false,
							   autoDelete: false,
							   arguments: null
							);

				channel.QueueDeclare(queue: "MyQueue1",
							   durable: false,
							   exclusive: false,
							   autoDelete: false,
							   arguments: null
							);

				// СВязываем очереди с обмеником и ключем
				channel.QueueBind(
								queue: "MyQueue",
								exchange: "my_exchange",
								routingKey: "my_key",
								arguments: null
							);

				channel.QueueBind(
								queue: "MyQueue1",
								exchange: "my_exchange",
								routingKey: "my_key",
								arguments: null
							);

				var body = Encoding.UTF8.GetBytes(message);

				// Отправляем сообщение в обменик
				channel.BasicPublish(exchange: "my_exchange",
							   routingKey: "my_key",
							   basicProperties: null,
							   body: body);
			}
		}
	}
}
