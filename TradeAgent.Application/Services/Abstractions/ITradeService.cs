using TradeAgent.Application.DTOs;

namespace TradeAgent.Application.Services.Abstractions
{
	public interface ITradeService
	{
		Task<TradeDto> RecordTradeAsync(CreateTradeRequest request, CancellationToken cancellationToken = default);
		Task<TradeDto?> GetTradeByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<IReadOnlyList<TradeDto>> GetAllTradesAsync(CancellationToken cancellationToken = default);
	}
}
