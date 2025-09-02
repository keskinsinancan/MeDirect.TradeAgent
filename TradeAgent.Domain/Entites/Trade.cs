using TradeAgent.Domain.Common;
using TradeAgent.Domain.Enums;
using TradeAgent.Domain.Events;
using TradeAgent.Domain.ValueObjects;

namespace TradeAgent.Domain.Entites
{
	public sealed class Trade : Entity
	{
		private Trade() { } // For EF Core
		public Asset Asset { get; private set; }
		public TradeSide Side { get; private set; }
		public decimal Quantity { get; private set; }
		public Money Price { get; private set; }
		public string CounterpartyId { get; private set; } = default!;
		public Guid UserId { get; private set; }
		public DateTime ExecutedAtUtc { get; private set; }

		private Trade(
			Asset asset,
			TradeSide side,
			decimal quantity,
			Money price,
			string counterpartyId,
			Guid userId,
			DateTime executedAtUtc)
		{
			Id = Guid.NewGuid();
			Asset = asset ?? throw new ArgumentNullException(nameof(asset));
			Side = side;
			Quantity = quantity;
			Price = price ?? throw new ArgumentNullException(nameof(price));
			CounterpartyId = !string.IsNullOrWhiteSpace(counterpartyId) ? counterpartyId : throw new ArgumentException("CounterpartyId is required.");
			UserId = userId;
			ExecutedAtUtc = executedAtUtc;

			AddDomainEvent(new TradeExecutedEvent(
				Id,
				Asset,
				Side,
				Quantity,
				Price,
				CounterpartyId,
				UserId,
				ExecutedAtUtc));
		}

		public static Trade Execute(
			Asset asset,
			TradeSide side,
			decimal quantity,
			Money price,
			string counterpartyId,
			Guid userId,
			DateTime executedAtUtc)
		{
			return new Trade(
				asset,
				side,
				quantity,
				price,
				counterpartyId,
				userId,
				executedAtUtc);
		}
	}
}
