using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace rabbitMQ_Consumer_Console
{
	class Program
	{
		static void Main(string[] args)
		{
			var factory = new ConnectionFactory() {
				UserName = "guest",
				Password = "guest",
				HostName = "10.10.11.18",
				Port = 5672,
				VirtualHost = "/"
			};
			//var factory = new ConnectionFactory() { Uri = new Uri("строка_подключения_облако") };
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				channel.QueueDeclare(queue: "MyQueue",
									 durable: false,
									 exclusive: false,
									 autoDelete: false,
									 arguments: null);

				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += (model, ea) =>
				{
					var body = ea.Body.ToArray();
					var message = Encoding.UTF8.GetString(body);
					Console.WriteLine(" [x] Received {0}", message);
				};
				channel.BasicConsume(queue: "MyQueue",
									 autoAck: true,
									 consumer: consumer);

				Console.WriteLine(" Press [enter] to exit.");
				Console.ReadLine();
			}
		}
	}
}
