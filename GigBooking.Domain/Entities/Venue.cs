using GigBooking.Domain.Errors;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Domain.Entities;

public class Venue : Entity<VenueId>
{
    public string Name { get; private set; } = string.Empty;

    private Venue(VenueId id, string name)  : base(id)
    {
        Name = name;
    }
    public static Result<Venue> Create(VenueId id, string name     )
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Venue>.Failure(DomainErrors.Venue.NoName);
        }
        return Result<Venue>.Success(new Venue(id, name));
    }
}
