using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Domain.Primitives;

public abstract class Entity : IEquatable<Entity>
{
    protected Entity(Guid id) => Id = id;

    //protected readonly List<IDomainEvent> _domainEvents = new();
    //public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public Guid Id { get; private init; }


    public static bool operator ==(Entity? left, Entity? right)
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
    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }

    // Generic version
    public override bool Equals(object? obj)
    {
        return Equals(obj as Entity);
    }
    public  bool Equals(Entity? other)
    {
        if (other == null)
        {
            return false;
        }
        if (other.GetType() != GetType())
        {
            return false;
        }
        if (other is not Entity entity)
        {
            return false;
        }
       return entity.Id == Id;
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
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
