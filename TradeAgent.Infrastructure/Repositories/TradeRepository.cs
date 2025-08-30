using Microsoft.EntityFrameworkCore;
using TradeAgent.Application.Abstractions.Repositories;
using TradeAgent.Domain.Entites;
using TradeAgent.Infrastructure.Data;

namespace TradeAgent.Infrastructure.Repositories
{
	public class TradeRepository(TradeAgentDbContext db) : ITradeRepository
	{
		private readonly TradeAgentDbContext _db = db;

		public async Task<Trade?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return await _db.Trades.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		}

		public async Task<IReadOnlyList<Trade>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await _db.Trades.ToListAsync(cancellationToken);
		}
		public async Task AddAsync(Trade trade, CancellationToken cancellationToken = default)
		{
			await _db.Trades.AddAsync(trade, cancellationToken);
		}
		public Task UpdateAsync(Trade trade, CancellationToken cancellationToken = default)
		{
			_db.Trades.Update(trade);
			return Task.CompletedTask;
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var trade = await _db.Trades.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
			if(trade != null)
			{
				_db.Trades.Remove(trade);
			}
		}						
	}
}
