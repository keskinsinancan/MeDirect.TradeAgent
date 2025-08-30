using TradeAgent.Domain.Common.Interfaces;

namespace TradeAgent.Domain.Common
{
	public abstract class Entity
	{
		private readonly List<IDomainEvent> _domainEvents = [];

		public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

		public Guid Id { get; protected set; }

		protected Entity() { }

		// Add domain event
		protected void AddDomainEvent(IDomainEvent domainEvent)
		{
			_domainEvents.Add(domainEvent);
		}

		// Clear domain events after publishing
		public void ClearDomainEvents() => _domainEvents.Clear();

		// Equality based on Id
		public override bool Equals(object? obj)
		{
			if (obj is not Entity other) return false;
			if (ReferenceEquals(this, other)) return true;
			if (GetType() != other.GetType()) return false;
			return Id == other.Id;
		}

		public override int GetHashCode() => Id.GetHashCode();
	}
}
