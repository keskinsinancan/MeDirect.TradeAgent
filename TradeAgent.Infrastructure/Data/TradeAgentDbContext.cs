using Microsoft.EntityFrameworkCore;
using TradeAgent.Domain.Entites;
using TradeAgent.Domain.OutBox;

namespace TradeAgent.Infrastructure.Data
{
	public class TradeAgentDbContext : DbContext
	{
		public TradeAgentDbContext(DbContextOptions<TradeAgentDbContext> options) : base(options) { }

		public DbSet<Trade> Trades => Set<Trade>();
		public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

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

				x.Property(t => t.Side).IsRequired();
				x.Property(t => t.Quantity).IsRequired();
				x.Property(t => t.Price).IsRequired();
				x.Property(t => t.Currency).IsRequired();
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
