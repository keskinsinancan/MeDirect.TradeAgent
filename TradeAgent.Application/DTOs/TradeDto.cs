
namespace TradeAgent.Application.DTOs
{
	public sealed class TradeDto
	{
		public Guid Id { get; init; }
		public string AssetName { get; init; } = default!;
		public string AssetSymbol { get; init; } = default!;
		public string Side { get; init; } = default!;
		public decimal Quantity { get; init; }
		public decimal Price { get; init; }
		public string Currency { get; init; } = default!;
		public string CounterpartyId { get; init; } = default!;
		public Guid UserId { get; init; } = default!;
		public DateTime ExecutedAtUtc { get; init; }
	}
}
