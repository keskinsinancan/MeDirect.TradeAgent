﻿using Microsoft.EntityFrameworkCore.Storage;
using TradeAgent.Application.Abstractions.UnitOfWork;
using TradeAgent.Infrastructure.Data;

namespace TradeAgent.Infrastructure.Repositories
{
	public class EfUnitOfWork(TradeAgentDbContext db) : IUnitOfWork, IDisposable
	{
		private readonly TradeAgentDbContext _db = db;
		private IDbContextTransaction? _transaction;

		public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
		{
			if (_transaction != null) return;
			_transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
		}

		public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
		{
			if (_transaction == null) return;

			await _db.SaveChangesAsync(cancellationToken);
			await _transaction.CommitAsync(cancellationToken);
			await _transaction.DisposeAsync();
			_transaction = null;
		}		

		public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
		{
			if (_transaction == null) return;
			await _transaction.RollbackAsync(cancellationToken);
			await _transaction.DisposeAsync();
			_transaction = null;
		}

		public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return _db.SaveChangesAsync(cancellationToken);
		}

		public void Dispose()
		{
			_transaction?.Dispose();
			_db?.Dispose();
		}
	}
}
