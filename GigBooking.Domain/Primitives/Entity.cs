using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Domain.Primitives;

public abstract class Entity<TId> : IEquatable<Entity<TId>> where TId: notnull
{
    public TId Id { get; private init; }
    
    protected Entity(TId id) => Id = id;

    //protected readonly List<IDomainEvent> _domainEvents = new();
    //public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();


    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (left is null && right is null)
        {
            return true;
        }
        if (left is null || right is null)
        {
            return false;
        }
        return left.Equals(right);
    }
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }

    // Generic version
    public override bool Equals(object? obj)
    {
        return Equals(obj as Entity<TId>);
    }
    public  bool Equals(Entity<TId>? other)
    {
        if (other == null)
        {
            return false;
        }
        if (other.GetType() != GetType())
        {
            return false;
        }
        if (other is not Entity<TId> entity)
        {
            return false;
        }
       return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }
    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? 0;
    }

    //protected void RaiseDomainEvent(IDomainEvent domainEvent)
    //{
    //    _domainEvents.Add(domainEvent);
    //}

    //public void ClearDomainEvents()
    //{
    //    _domainEvents.Clear();
    //}
}
