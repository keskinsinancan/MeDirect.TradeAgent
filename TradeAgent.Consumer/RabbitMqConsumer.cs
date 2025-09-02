using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using TradeAgent.Logging;

namespace TradeAgent.Consumer
{
	public class RabbitMqConsumer(IOptions<RabbitMqOptions> options, ILogStore logStore, ILogger<RabbitMqConsumer> logger)
	{
		private readonly RabbitMqOptions _options = options.Value;
		private readonly ILogStore _logStore = logStore;
		private readonly ILogger<RabbitMqConsumer> _logger = logger;
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
				var correlationId = ea.BasicProperties?.CorrelationId ?? Guid.NewGuid().ToString();

				try
				{
					_logger.LogInformation("Message received from RabbitMQ. CorrelationId={CorrelationId}, Message={Message}", correlationId, message);
					await channel.BasicAckAsync(ea.DeliveryTag, false);
					_logStore.Add($"[CONSUMER] CorrelationId={correlationId} | Message consumed: {message}");
				}
				catch(Exception ex) 
				{
					_logger.LogError(ex,"Error processing message. CorrelationId={CorrelationId}, Message={Message}", correlationId, message);
					_logStore.Add($"[CONSUMER ERROR] CorrelationId={correlationId} | {ex.Message}");
					await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
				}
			};

			string consumerTag = await channel.BasicConsumeAsync(_options.QueueName, false, consumer);
		}
	}
}
