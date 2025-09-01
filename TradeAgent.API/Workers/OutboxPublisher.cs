using Microsoft.EntityFrameworkCore;
using TradeAgent.Domain.Enums;
using TradeAgent.Infrastructure.Data;
using TradeAgent.Infrastructure.Messaging;
using TradeAgent.Logging;

namespace TradeAgent.API.Workers
{
	public class OutboxPublisher(
		IServiceScopeFactory scopeFactory,
		RabbitMqPublisher publisher,
		ILogger<OutboxPublisher> logger,
		DistributedDemoLogStore logStore ) : BackgroundService
	{
		private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
		private readonly RabbitMqPublisher _publisher = publisher;
		private readonly ILogger<OutboxPublisher> _logger = logger;
		private readonly DistributedDemoLogStore _logStore = logStore;

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				using var scope = _scopeFactory.CreateScope();
				var db = scope.ServiceProvider.GetRequiredService<TradeAgentDbContext>();

				var messages = await db.OutboxMessages
					.Where(m => m.Status == OutboxMessageStatus.Pending)
					.ToListAsync(stoppingToken);

				foreach (var message in messages)
				{
					try
					{
						await _publisher.Publish(message.Payload, message.Type);
						message.MarkProcessed(DateTime.UtcNow);
						_logger.LogInformation("Published message {Id}", message.Id);
						_logStore.Add($"Published outbox event: {message.Id}");
					}
					catch (Exception ex)
					{
						message.MarkFailed(ex.Message);
						_logger.LogError(ex, "Failed to publish message {Id}", message.Id);
						_logStore.Add($"Error publishing outbox event: {message.Id}");
					}
				}

				await db.SaveChangesAsync(stoppingToken);
				await Task.Delay(1000, stoppingToken);
			}
		}
	}
}
