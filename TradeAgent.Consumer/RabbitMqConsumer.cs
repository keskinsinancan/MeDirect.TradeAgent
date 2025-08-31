using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;

namespace TradeAgent.Consumer
{
	public class RabbitMqConsumer
	{
		private readonly string _hostname;
		private readonly string _queueName;

		public RabbitMqConsumer(string hostname, string queueName)
		{
			_hostname = hostname;
			_queueName = queueName;
		}

		public async Task Start()
		{
			var factory = new ConnectionFactory
			{
				HostName = "localhost",
				Port = 5672,
				UserName = "admin",
				Password = "admin123"
			};

			IConnection conn = await factory.CreateConnectionAsync();

			// Create channel
			var channel = await conn.CreateChannelAsync();

			await channel.ExchangeDeclareAsync("trades", ExchangeType.Direct);
			await channel.QueueDeclareAsync("trade_executed", true, false, false, null);
			await channel.QueueBindAsync("trade_executed", "trades", "TradeExecutedEvent", null);

			var consumer = new AsyncEventingBasicConsumer(channel);
			consumer.ReceivedAsync += async (ch, ea) =>
			{
				var body = ea.Body.ToArray();
				// copy or deserialise the payload
				// and process the message
				// ...
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine($"[x] Received: {message}");

				await channel.BasicAckAsync(ea.DeliveryTag, false);
			};
			// this consumer tag identifies the subscription
			// when it has to be cancelled
			string consumerTag = await channel.BasicConsumeAsync("trade_executed", false, consumer);
		}
	}
}
