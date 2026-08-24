using GigBooking.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Domain.Entities;

public class Venue : Entity
{
    public string Name { get; private set; } = string.Empty;

    private Venue(Guid id, string name)  : base(id)
    {
        Name = name;
    }
    public static Result<Venue> Create(Guid id, string name     )
    {
        if (string.IsNullOrEmpty(name))
        {
            return Result<Venue>.Failure(new Error("InvalidVenueName", "Venue name cannot be null or empty."));
        }
        return Result<Venue>.Success(new Venue(id, name));
    }
}
