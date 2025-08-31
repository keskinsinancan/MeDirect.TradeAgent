using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TradeAgent.Infrastructure.Settings;

namespace TradeAgent.Infrastructure.Messaging
{
	public class RabbitMqPublisher(IOptions<RabbitMqOptions> options)
	{
		private readonly RabbitMqOptions _options = options.Value;

		public async Task Publish<T>(T message, string routingKey) where T : class
		{
			var factory = new ConnectionFactory
			{
				UserName = _options.Username,
				Password = _options.Password,
				HostName = _options.Host,
				Port = _options.Port,
			};

			IConnection conn = await factory.CreateConnectionAsync();

			// Create channel
			var channel = await conn.CreateChannelAsync();

			await channel.ExchangeDeclareAsync(_options.ExchangeName, ExchangeType.Direct);
			await channel.QueueDeclareAsync(_options.QueueName, true, false, false, null);
			await channel.QueueBindAsync(_options.QueueName, _options.ExchangeName, routingKey, null);

			string json;

			if (message is string s)
				json = s;
			else
				json = JsonSerializer.Serialize(message);

			var body = Encoding.UTF8.GetBytes(json);

			await channel.BasicPublishAsync(
			  exchange: _options.ExchangeName,
			  routingKey: routingKey,
			  body: body
		  );
		}
	}
}