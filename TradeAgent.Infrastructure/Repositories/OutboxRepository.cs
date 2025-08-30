using Microsoft.EntityFrameworkCore;
using TradeAgent.Application.Abstractions.Repositories;
using TradeAgent.Domain.OutBox;
using TradeAgent.Infrastructure.Data;
using TradeAgent.Domain.Enums;

namespace TradeAgent.Infrastructure.Repositories
{
	public class OutboxRepository(TradeAgentDbContext db) : IOutboxRepository
	{
		private readonly TradeAgentDbContext _db = db;

		public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
		{
			await _db.OutboxMessages.AddAsync(message, cancellationToken);
		}

		public async Task<OutboxMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return await _db.OutboxMessages.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		}

		public async Task<IReadOnlyList<OutboxMessage>> GetPendingMessagesAsync(CancellationToken cancellationToken = default)
		{
			return await _db.OutboxMessages
				.Where(x => x.Status == OutboxMessageStatus.Pending)
				.OrderBy(x => x.OccurredOnUtc)
				.ToListAsync(cancellationToken);
		}

		public Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default)
		{
			_db.OutboxMessages.Update(message);
			return Task.CompletedTask;
		}
	}
}
