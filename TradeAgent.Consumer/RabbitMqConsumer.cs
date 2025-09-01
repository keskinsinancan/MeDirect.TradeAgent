using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using TradeAgent.Logging;

namespace TradeAgent.Consumer
{
	public class RabbitMqConsumer(IOptions<RabbitMqOptions> options, DistributedDemoLogStore logStore)
	{
		private readonly RabbitMqOptions _options = options.Value;
		private readonly DistributedDemoLogStore _logStore = logStore;

		public async Task Start()
		{
			var factory = new ConnectionFactory
			{
				UserName = _options.Username,
				Password = _options.Password,
				HostName = _options.Host,
				Port = _options.Port,
			};

			IConnection conn = await factory.CreateConnectionAsync();

			var channel = await conn.CreateChannelAsync();
			await channel.ExchangeDeclareAsync(_options.ExchangeName, ExchangeType.Direct);
			await channel.QueueDeclareAsync(_options.QueueName, true, false, false, null);
			await channel.QueueBindAsync(_options.QueueName, _options.ExchangeName, _options.RoutingKey, null);

			var consumer = new AsyncEventingBasicConsumer(channel);
			consumer.ReceivedAsync += async (ch, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine($"[x] Received: {message}");
				await channel.BasicAckAsync(ea.DeliveryTag, false);
				_logStore.Add($"Consumed message: {message}");
			};

			string consumerTag = await channel.BasicConsumeAsync(_options.QueueName, false, consumer);
		}
	}
}
