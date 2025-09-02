using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using TradeAgent.Logging;

namespace TradeAgent.Consumer
{
	public class RabbitMqConsumer(
		IOptions<RabbitMqOptions> options,
		ILogStore logStore,
		ILogger<RabbitMqConsumer> logger) : BackgroundService
	{
		private readonly RabbitMqOptions _options = options.Value;
		private readonly ILogStore _logStore = logStore;
		private readonly ILogger<RabbitMqConsumer> _logger = logger;

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
			await channel.ExchangeDeclareAsync(_options.ExchangeName, ExchangeType.Direct, cancellationToken: stoppingToken);
			await channel.QueueDeclareAsync(_options.QueueName, true, false, false, null, cancellationToken: stoppingToken);
			await channel.QueueBindAsync(_options.QueueName, _options.ExchangeName, _options.RoutingKey, null, cancellationToken: stoppingToken);

			var consumer = new AsyncEventingBasicConsumer(channel);
			consumer.ReceivedAsync += async (ch, ea) => await HandleMessageAsync(channel, ea);

			string consumerTag = await channel.BasicConsumeAsync(_options.QueueName, false, consumer);

			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(1000, stoppingToken);
			}
		}

		private async Task HandleMessageAsync(IChannel channel, BasicDeliverEventArgs ea)
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
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing message. CorrelationId={CorrelationId}, Message={Message}", correlationId, message);
				_logStore.Add($"[CONSUMER ERROR] CorrelationId={correlationId} | {ex.Message}");
				await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
			}
		}
	}
}
