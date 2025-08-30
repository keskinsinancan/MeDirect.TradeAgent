using TradeAgent.Domain.Common;
using TradeAgent.Domain.Enums;
using TradeAgent.Domain.Events;
using TradeAgent.Domain.ValueObjects;

namespace TradeAgent.Domain.Entites
{
	public sealed class Trade : Entity
	{
		public Asset Asset{ get; private set; }
		public TradeSide Side { get; private set; }
		public decimal Quantity { get; private set; }
		//todo : Make a money value object for price
		public decimal Price { get; private set; }
		public string Currency { get; private set; } = default!;
		public string CounterpartyId { get; private set; } = default!;
		public DateTime ExecutedAtUtc { get; private set; }

		private Trade() { }

		private Trade(string assetName, string assetSymbol, TradeSide side, decimal quantity, decimal price, string currency, string counterpartyId)
		{
			Id = Guid.NewGuid();
			Asset = Asset.Create(assetName, assetSymbol);
			Side = side;
			Quantity = quantity;
			Price = price;
			Currency = currency;
			CounterpartyId = counterpartyId;
			ExecutedAtUtc = DateTime.UtcNow;

			AddDomainEvent(new TradeExecutedDomainEvent(Id, Asset, Side, Quantity, Price, Currency, CounterpartyId, ExecutedAtUtc));
		}

		public static Trade Execute(string assetName, string assetSymbol, TradeSide side, decimal quantity, decimal price, string currency, string counterpartyId)
			=> new(assetName, assetSymbol, side, quantity, price, currency, counterpartyId);
	}
}
