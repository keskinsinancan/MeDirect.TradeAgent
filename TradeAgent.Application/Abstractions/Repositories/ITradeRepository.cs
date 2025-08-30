using TradeAgent.Domain.Entites;

namespace TradeAgent.Application.Abstractions.Repositories
{
	public interface ITradeRepository
	{
		Task<Trade?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<IReadOnlyList<Trade>> GetAllAsync(CancellationToken cancellationToken = default);
		Task AddAsync(Trade trade, CancellationToken cancellationToken = default);
		Task UpdateAsync(Trade trade, CancellationToken cancellationToken = default);
		Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
	}
}
