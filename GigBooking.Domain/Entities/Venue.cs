using GigBooking.Domain.Primitives;
using GigBooking.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Domain.Entities;

public class Venue : Entity<VenueId>
{
    public string Name { get; private set; } = string.Empty;
    public Location Location { get; private set; } = null!;

    private Venue() : base(default!)
    {

    }
    private Venue(VenueId id, string name, Location location)  : base(id)
    {
        Name = name;
        Location = location;
    }
    public static Result<Venue> Create(VenueId id, string name, Location location)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Venue>.Failure(Errors.Venue.NoName);
        }

        if (location is null)
        {
            return Result<Venue>.Failure(Errors.Venue.NullLocation);
        }
        return Result<Venue>.Success(new Venue(id, name, location));
    }
}
