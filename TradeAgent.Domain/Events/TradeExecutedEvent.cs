using TradeAgent.Domain.Enums;
using TradeAgent.Domain.ValueObjects;

namespace TradeAgent.Domain.Events
{
	public sealed record TradeExecutedEvent
		(	Guid TradeId,
			Asset Asset,
			TradeSide Side,
			decimal Quantity,
			Money Price,
			string CounterpartyId,
			Guid UserId,
			DateTime ExecutedAtUtc
		) : DomainEvent;
}
