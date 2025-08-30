using Microsoft.EntityFrameworkCore;
using TradeAgent.Domain.Entites;

namespace TradeAgent.Infrastructure.Data
{
	public class TradeAgentDbContext : DbContext
	{
		public TradeAgentDbContext(DbContextOptions<TradeAgentDbContext> options) : base(options) { }

		public DbSet<Trade> Trades => Set<Trade>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Trade>(b =>
			{
				b.HasKey(t => t.Id);

				b.OwnsOne(t => t.Asset, i =>
				{
					i.Property(p => p.Name).HasColumnName("InstrumentName").IsRequired();
					i.Property(p => p.Symbol).HasColumnName("InstrumentSymbol").IsRequired();
				});

				b.Property(t => t.Side).IsRequired();
				b.Property(t => t.Quantity).IsRequired();
				b.Property(t => t.Price).IsRequired();
				b.Property(t => t.Currency).IsRequired();
				b.Property(t => t.CounterpartyId).IsRequired();
				b.Property(t => t.ExecutedAtUtc).IsRequired();
			});
		}
	}
}
