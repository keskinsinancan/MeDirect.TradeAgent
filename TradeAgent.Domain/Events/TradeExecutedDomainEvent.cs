﻿using TradeAgent.Domain.Enums;
using TradeAgent.Domain.ValueObjects;

namespace TradeAgent.Domain.Events
{
	public sealed record TradeExecutedDomainEvent
		(	Guid TradeId,
			Asset Asset,
			TradeSide Side,
			decimal Quantity,
			decimal Price,
			string Currency,
			string CounterpartyId,
			Guid UserId,
			DateTime ExecutedAtUtc
		) : DomainEvent;
}
