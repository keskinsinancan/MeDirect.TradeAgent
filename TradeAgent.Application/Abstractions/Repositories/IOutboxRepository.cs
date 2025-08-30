using TradeAgent.Domain.OutBox;

namespace TradeAgent.Application.Abstractions.Repositories
{
	public interface IOutboxRepository
	{
		Task<OutboxMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<IReadOnlyList<OutboxMessage>> GetPendingMessagesAsync(CancellationToken cancellationToken = default);
		Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);
		Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default);
	}
}
