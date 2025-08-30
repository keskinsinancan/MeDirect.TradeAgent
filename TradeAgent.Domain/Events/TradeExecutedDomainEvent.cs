using TradeAgent.Domain.Common;
using TradeAgent.Domain.Enums;
using TradeAgent.Domain.ValueObjects;

namespace TradeAgent.Domain.Events
{
	public sealed record TradeExecutedDomainEvent
		(	Guid TradeId,
			Asset Instrument,
			TradeSide Side,
			decimal Quantity,
			decimal Price,
			string Currency,
			string CounterpartyId,
			DateTime ExecutedAtUtc
		) : DomainEvent;
}
