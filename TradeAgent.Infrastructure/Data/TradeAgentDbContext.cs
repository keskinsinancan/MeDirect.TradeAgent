using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TradeAgent.Domain.Common;
using TradeAgent.Domain.Entites;
using TradeAgent.Domain.OutBox;

namespace TradeAgent.Infrastructure.Data
{
	public class TradeAgentDbContext : DbContext
	{
		public TradeAgentDbContext(DbContextOptions<TradeAgentDbContext> options) : base(options) { }
		public DbSet<Trade> Trades => Set<Trade>();
		public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var domainEvents = ChangeTracker
				.Entries<Entity>()
				.SelectMany(e => e.Entity.DomainEvents)
				.ToList();

			foreach (var entry in ChangeTracker.Entries<Entity>())
			{
				entry.Entity.ClearDomainEvents();
			}

			foreach (var @event in domainEvents)
			{
				var outboxMessage = new OutboxMessage(
					id: Guid.NewGuid(),
					occurredOnUtc: @event.OccuredOnUtc,
					type: @event.GetType().Name,
					payload: JsonSerializer.Serialize(@event, @event.GetType())
				);

				await OutboxMessages.AddAsync(outboxMessage, cancellationToken);
			}

			return await base.SaveChangesAsync(cancellationToken);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Trade>(x =>
			{
				x.ToTable("trades");

				x.HasKey(t => t.Id);

				x.OwnsOne(t => t.Asset, i =>
				{
					i.Property(p => p.Name).HasColumnName("AssetName").IsRequired();
					i.Property(p => p.Symbol).HasColumnName("AssetSymbol").IsRequired();
				});

				x.OwnsOne(t => t.Price, p =>
				{
					p.Property(m => m.Amount).HasColumnName("PriceAmount").IsRequired();
					p.Property(m => m.Currency).HasColumnName("PriceCurrency").IsRequired();
				});

				x.Property(t => t.Side).IsRequired();
				x.Property(t => t.Quantity).IsRequired();
				x.Property(t => t.CounterpartyId).IsRequired();
				x.Property(t => t.ExecutedAtUtc).IsRequired();
			});

			modelBuilder.Entity<OutboxMessage>(x =>
			{
				x.ToTable("outboxmessages");
				x.HasKey(o => o.Id);

				x.Property(o => o.Type)
				 .IsRequired();

				x.Property(o => o.Payload)
				 .IsRequired();

				x.Property(o => o.Status)
				 .IsRequired();
			});
		}
	}
}
