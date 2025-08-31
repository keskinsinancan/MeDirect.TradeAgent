using TradeAgent.Application.Abstractions.Repositories;
using TradeAgent.Application.Abstractions.UnitOfWork;
using TradeAgent.Application.DTOs;
using TradeAgent.Application.Services.Abstractions;
using TradeAgent.Domain.Entites;
using TradeAgent.Domain.Enums;
using TradeAgent.Domain.ValueObjects;

namespace TradeAgent.Application.Services
{
	public sealed class TradeService(ITradeRepository tradeRepository, IUnitOfWork unitOfWork) : ITradeService
	{
		private readonly ITradeRepository _tradeRepository = tradeRepository;
		private readonly IUnitOfWork _unitOfWork = unitOfWork;

		public async Task<TradeDto> RecordTradeAsync(CreateTradeRequest request, CancellationToken cancellationToken = default)
		{
			var trade = Trade.Execute(
					asset: Asset.Create(request.AssetName, request.AssetSymbol),
					side: Enum.Parse<TradeSide>(request.Side, ignoreCase: true),
					quantity: request.Quantity,
					price: request.Price,
					currency: request.Currency,
					counterpartyId: request.CounterpartyId,
					userId: request.UserId,
					executedAtUtc: DateTime.UtcNow
				);

			await _tradeRepository.AddAsync(trade, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return MapToDto(trade);
		}	

		public async Task<IReadOnlyList<TradeDto>> GetAllTradesAsync(CancellationToken cancellationToken = default)
		{
			var trades = await _tradeRepository.GetAllAsync(cancellationToken);
			return [.. trades.Select(MapToDto)];
		}

		public async Task<TradeDto?> GetTradeByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var trade = await _tradeRepository.GetByIdAsync(id, cancellationToken);
			return trade is null ? null : MapToDto(trade);
		}

		private static TradeDto MapToDto(Trade trade) => new()
		{
			Id = trade.Id,
			AssetName = trade.Asset.Name,
			AssetSymbol = trade.Asset.Symbol,
			Side = trade.Side.ToString(),
			Quantity = trade.Quantity,
			Price = trade.Price,
			Currency = trade.Currency,
			CounterpartyId = trade.CounterpartyId,
			UserId = trade.UserId,
			ExecutedAtUtc = trade.ExecutedAtUtc
		};
	}
}
