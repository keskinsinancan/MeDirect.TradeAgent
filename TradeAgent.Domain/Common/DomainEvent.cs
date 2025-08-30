using TradeAgent.Domain.Common.Abstractions;

namespace TradeAgent.Domain.Common
{
	public abstract record DomainEvent : IDomainEvent
	{
		public DateTime OccuredOnUtc { get; } = DateTime.UtcNow;
	}
}
