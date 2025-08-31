namespace TradeAgent.Domain.Events
{
	public interface IDomainEvent
	{
		DateTime OccuredOnUtc { get; }
	}
}
