namespace TradeAgent.Domain.Common.Abstractions
{
	public interface IDomainEvent
	{
		DateTime OccuredOnUtc { get; }
	}
}
