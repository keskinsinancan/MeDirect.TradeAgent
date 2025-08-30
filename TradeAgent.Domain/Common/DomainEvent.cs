using TradeAgent.Domain.Common.Interfaces;

namespace TradeAgent.Domain.Common
{
	public abstract record DomainEvent : IDomainEvent
	{
		public DateTime OccuredOnUtc { get; } = DateTime.UtcNow;
	}
}
