using TradeAgent.Domain.Common;
using TradeAgent.Domain.Enums;
using TradeAgent.Domain.Events;
using TradeAgent.Domain.ValueObjects;

namespace TradeAgent.Domain.Entites
{
	public sealed class Trade : Entity
	{
		public Asset Asset { get; private set; }
		public TradeSide Side { get; private set; }
		public decimal Quantity { get; private set; }
		// TODO: Replace with a Money value object later
		public decimal Price { get; private set; }
		public string Currency { get; private set; } = default!;
		public string CounterpartyId { get; private set; } = default!;
		public DateTime ExecutedAtUtc { get; private set; }
		private Trade() { } // For EF Core

		private Trade(
			Asset asset,
			TradeSide side,
			decimal quantity,
			decimal price,
			string currency,
			string counterpartyId,
			DateTime executedAtUtc)
		{
			Id = Guid.NewGuid();
			Asset = asset ?? throw new ArgumentNullException(nameof(asset));
			Side = side;
			Quantity = quantity;
			Price = price;
			Currency = !string.IsNullOrWhiteSpace(currency) ? currency : throw new ArgumentException("Currency is required.");
			CounterpartyId = !string.IsNullOrWhiteSpace(counterpartyId) ? counterpartyId : throw new ArgumentException("CounterpartyId is required.");
			ExecutedAtUtc = executedAtUtc;

			AddDomainEvent(new TradeExecutedDomainEvent(
				Id,
				Asset,
				Side,
				Quantity,
				Price,
				Currency,
				CounterpartyId,
				ExecutedAtUtc));
		}

		public static Trade Execute(
			Asset asset,
			TradeSide side,
			decimal quantity,
			decimal price,
			string currency,
			string counterpartyId,
			DateTime executedAtUtc)
		{
			return new Trade(
				asset,
				side,
				quantity,
				price,
				currency,
				counterpartyId,
				executedAtUtc);
		}
	}
}
