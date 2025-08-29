namespace TradeAgent.Domain.Common.Interfaces
{
	public interface IDomainEvent
	{
		DateTime OccuredOnUtc { get; }
	}
}
