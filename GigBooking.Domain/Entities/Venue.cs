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
    public bool IsActive { get; private set; } = true;
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

    public Result ChangeName(string newName)
    {

        if (!IsActive)
        {
            return Result.Failure(Errors.Venue.AlreadyInactive);
        }
        if (string.IsNullOrWhiteSpace(newName))
        {
            return Result.Failure(Errors.Venue.NoName);
        }
        var trimmed = newName.Trim();
        if (string.Equals(Name, trimmed))
        {
            return Result.Success();
        }
        Name = trimmed;
        return Result.Success();
    }
    public Result ChangeLocation(Location newLocation)
    {
        if (!IsActive)
        {
            return Result.Failure(Errors.Venue.AlreadyInactive);
        }
        if (newLocation is null)
        {
            return Result.Failure(Errors.Venue.NullLocation);
        }
        Location = newLocation;
        return Result.Success();
    }
    public Result DeActivate()
    {
        if (IsActive == false)
        {
            return Result.Failure(Errors.Venue.AlreadyInactive);
        }

        IsActive = false;
        return Result.Success();
    }

}
