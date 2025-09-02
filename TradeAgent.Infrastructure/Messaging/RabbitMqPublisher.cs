using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TradeAgent.Infrastructure.Settings;
using TradeAgent.Logging;

namespace TradeAgent.Infrastructure.Messaging
{
	public class RabbitMqPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqPublisher> logger, ILogStore logStore)
	{
		private readonly RabbitMqOptions _options = options.Value;
		private readonly ILogger<RabbitMqPublisher> _logger = logger;
		private readonly ILogStore _logStore = logStore;

		public async Task Publish<T>(T message, string routingKey) where T : class
		{
			try
			{
				var factory = new ConnectionFactory
				{
					UserName = _options.Username,
					Password = _options.Password,
					HostName = _options.Host,
					Port = _options.Port,
				};

				using var conn = await factory.CreateConnectionAsync();
				using var channel = await conn.CreateChannelAsync();

				await channel.ExchangeDeclareAsync(_options.ExchangeName, ExchangeType.Direct);
				await channel.QueueDeclareAsync(_options.QueueName, true, false, false, null);
				await channel.QueueBindAsync(_options.QueueName, _options.ExchangeName, routingKey, null);

				string json = message is string s ? s : JsonSerializer.Serialize(message);
				var body = Encoding.UTF8.GetBytes(json);

				await channel.BasicPublishAsync(
					exchange: _options.ExchangeName,
					routingKey: routingKey,
					body: body
				);

				_logger.LogInformation("Message published to RabbitMQ. RoutingKey={RoutingKey}, Message={Message}", routingKey, json);
				_logStore.Add($"[PUBLISHER] RoutingKey={routingKey} | Message published: {json}");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to publish message to RabbitMQ. RoutingKey={RoutingKey}, Message={Message}", routingKey, message);
				_logStore.Add($"[PUBLISHER ERROR] RoutingKey={routingKey} | {ex.Message}");
				throw;
			}
		}
	}
}