using TradeAgent.Infrastructure.Enums;

namespace TradeAgent.Infrastructure.Outbox
{
	public sealed class OutboxMessage
	{
		public Guid Id { get; private set; }
		public DateTime OccurredOnUtc { get; private set; }
		public string Type { get; private set; } = default!;
		public string Payload { get; private set; } = default!;
		public OutboxMessageStatus Status { get; private set; }
		public DateTime? ProcessedOnUtc { get; private set; }
		public string? Error { get; private set; }
		private OutboxMessage() { }
		public OutboxMessage(Guid id, DateTime occurredOnUtc, string type, string payload)
		{
			Id = id;
			OccurredOnUtc = occurredOnUtc;
			Type = type;
			Payload = payload;
			Status = OutboxMessageStatus.Pending;
		}
		public void MarkProcessing() => Status = OutboxMessageStatus.Processing;
		public void MarkProcessed(DateTime processedOnUtc)
		{
			Status = OutboxMessageStatus.Processed;
			ProcessedOnUtc = processedOnUtc;
		}
		public void MarkFailed(string error)
		{
			Status = OutboxMessageStatus.Failed;
			Error = error;
		}
	}
}
