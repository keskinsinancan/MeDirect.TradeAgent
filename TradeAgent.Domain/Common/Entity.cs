﻿using TradeAgent.Domain.Events;

namespace TradeAgent.Domain.Common
{
	public abstract class Entity
	{
		public Guid Id { get; protected set; }
		private readonly List<IDomainEvent> _domainEvents = [];
		public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

		protected Entity() { }
		protected void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);
		public void ClearDomainEvents() => _domainEvents.Clear();

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
