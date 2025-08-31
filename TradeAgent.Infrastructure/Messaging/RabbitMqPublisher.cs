using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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

			// Async connection
			await using var connection = await factory.CreateConnectionAsync();

			// Channel oluşturma (senkron)
			using var channel = connection.CreateModel();  // <- sadece bu var, async değil

			// Exchange
			channel.ExchangeDeclare(exchange: _options.Exchange, type: ExchangeType.Fanout, durable: true);

			// Mesajı serileştir
			var json = JsonSerializer.Serialize(message);
			var body = Encoding.UTF8.GetBytes(json);

			// Yayınla
			channel.BasicPublish(
				exchange: _options.Exchange,
				routingKey: routingKey,
				basicProperties: null,
				body: body
			);
		}
	}
}
