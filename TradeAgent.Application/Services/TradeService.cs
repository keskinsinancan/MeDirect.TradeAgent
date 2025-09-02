using Microsoft.Extensions.Logging;
using TradeAgent.Application.Abstractions.Repositories;
using TradeAgent.Application.Abstractions.UnitOfWork;
using TradeAgent.Application.DTOs;
using TradeAgent.Application.Services.Abstractions;
using TradeAgent.Domain.Entites;
using TradeAgent.Domain.Enums;
using TradeAgent.Domain.ValueObjects;
using TradeAgent.Logging;

namespace TradeAgent.Application.Services
{
	public sealed class TradeService(ITradeRepository tradeRepository, IUnitOfWork unitOfWork,
										ILogger<TradeService> logger, DistributedDemoLogStore logStore) : ITradeService
	{
		private readonly ITradeRepository _tradeRepository = tradeRepository;
		private readonly IUnitOfWork _unitOfWork = unitOfWork;
		private readonly ILogger<TradeService> _logger = logger;
		private readonly DistributedDemoLogStore _logStore = logStore;
		public async Task<TradeDto> RecordTradeAsync(CreateTradeRequest request, CancellationToken cancellationToken = default)
		{
			var tradeId = Guid.NewGuid();
			try
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

				_logger.LogInformation("Executing trade. TradeId={TradeId}, Asset={Asset}, Quantity={Quantity}", trade.Id, trade.Asset.Name, trade.Quantity);
				_logStore.Add($"[TRADE] Executing trade. TradeId={trade.Id}, Asset={trade.Asset.Name}, Quantity={trade.Quantity}");

				await _tradeRepository.AddAsync(trade, cancellationToken);
				await _unitOfWork.SaveChangesAsync(cancellationToken);

				_logger.LogInformation("Trade recorded successfully. TradeId={TradeId}", trade.Id);
				_logStore.Add($"[TRADE] Trade recorded successfully. TradeId={trade.Id}");

				return MapToDto(trade);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to record trade. TradeId={TradeId}", tradeId);
				_logStore.Add($"[TRADE ERROR] TradeId={tradeId} | {ex.Message}");
				throw;
			}
		}

		public async Task<IReadOnlyList<TradeDto>> GetAllTradesAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				var trades = await _tradeRepository.GetAllAsync(cancellationToken);
				_logger.LogInformation("Retrieved all trades. Count={Count}", trades.Count);
				return trades.Select(MapToDto).ToList();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to retrieve all trades");
				_logStore.Add($"[TRADE ERROR] Failed to retrieve all trades | {ex.Message}");
				throw;
			}
		}

		public async Task<TradeDto?> GetTradeByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			try
			{
				var trade = await _tradeRepository.GetByIdAsync(id, cancellationToken);
				if (trade is null) return null;

				_logger.LogInformation("Retrieved trade by Id. TradeId={TradeId}", id);
				return MapToDto(trade);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to retrieve trade by Id. TradeId={TradeId}", id);
				_logStore.Add($"[TRADE ERROR] TradeId={id} | {ex.Message}");
				throw;
			}
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
