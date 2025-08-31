namespace TradeAgent.Domain.Events
{
	public abstract record DomainEvent : IDomainEvent
	{
		public DateTime OccuredOnUtc { get; protected init; } = DateTime.UtcNow;
	}
}
