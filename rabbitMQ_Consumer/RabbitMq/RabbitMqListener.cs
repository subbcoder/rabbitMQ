using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Diagnostics;
using System;

namespace rabbitMQ_Consumer.RabbitMq
{
	public class RabbitMqListener : BackgroundService
	{
		private IConnection _connection;
		private IModel _channel;
		public RabbitMqListener()
		{
			// Не забудьте вынести значения "localhost" и "MyQueue"
			// в файл конфигурации
			var factory = new ConnectionFactory {
				UserName = "user",
				Password = "1234",
				HostName = "10.10.11.18",
				Port = 5672,
				VirtualHost = "/forUser",
				AutomaticRecoveryEnabled = true
			};
			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();

			string sExchangeName = "my_exchange";
			string sQueueName = "MyQueue";
			string sКoutingKey = "my_key";

			//_channel.ExchangeDeclarePassive(sExchangeName);


			//QueueDeclareOk ok = _channel.QueueDeclarePassive(sQueueName);

			//if (ok.MessageCount > 0)
			//{
			//	// Bind the queue to the exchange
			//	_channel.QueueBind(
			//			queue: sQueueName,
			//			exchange: sExchangeName,
			//			routingKey: sКoutingKey,
			//			arguments: null
			//		);
			//}
			//else
            {
				_channel.QueueDeclare(
						queue: sQueueName, 
						durable: false, 
						exclusive: false, 
						autoDelete: false, 
						arguments: null
					);
				// Bind the queue to the exchange
				//_channel.QueueBind(
				//		queue: sQueueName,
				//		exchange: sExchangeName,
				//		routingKey: sКoutingKey,
				//		arguments: null
				//	);

			}
			// Хотя это все извращение.
			// queue.declare — это идемпотентная операция. Итак, если вы запустите его один, два, N раз, результат все равно будет таким же.

		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			stoppingToken.ThrowIfCancellationRequested();

			var consumer = new EventingBasicConsumer(_channel);

			consumer.Received += OnConsumerReceived;
			//consumer.Received += (ch, ea) =>
			//{
			//	var content = Encoding.UTF8.GetString(ea.Body.ToArray());

			//	// Каким-то образом обрабатываем полученное сообщение
			//	Console.WriteLine($"Получено сообщение: {content}");

			//	_channel.BasicAck(ea.DeliveryTag, false);
			//};

			_channel.BasicConsume("MyQueue", false, consumer);

			return Task.CompletedTask;
		}

        private void OnConsumerReceived(object sender, BasicDeliverEventArgs ea)
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());

            // Каким-то образом обрабатываем полученное сообщение
            Console.WriteLine($"Получено сообщение: {content}");

            _channel.BasicAck(ea.DeliveryTag, false);
        }

		public override void Dispose()
		{
			_channel.Close();
			_connection.Close();
			base.Dispose();
		}
	}
}
