using GigBooking.Domain.Errors;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GigBooking.Domain.Entities;

public sealed class Artist : Entity<ArtistId>
{
    public string Name { get; private set; } = string.Empty;
    private Artist(ArtistId id, string name ) : base(id)
    {
        Name = name;
    } 

    public static Result<Artist> Create(ArtistId id, string name)
    {

        if (string.IsNullOrEmpty(name))
        {
            return Result<Artist>.Failure(DomainErrors.Artist.NoName);
        }
        return Result<Artist>.Success(new Artist(id, name));
    }
}
