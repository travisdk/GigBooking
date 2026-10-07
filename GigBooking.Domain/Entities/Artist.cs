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
    public bool IsActive { get; private set; } = true;
    private Artist(ArtistId id, string name ) : base(id)
    {
        Name = name;
    } 

    public static Result<Artist> Create(ArtistId id, string name)
    {

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Artist>.Failure(Errors.Artist.NoName);
        }
        return Result<Artist>.Success(new Artist(id, name.Trim()));
    }

    public Result ChangeName(string newName) {

        if (!IsActive)
        {
            return Result.Failure(Errors.Artist.AlreadyInactive);
        }
        if (string.IsNullOrWhiteSpace(newName))
        {
            return Result.Failure(Errors.Artist.NoName);
        }
        var trimmed = newName.Trim();
        if (string.Equals(Name, trimmed ))
        {
            return Result.Success();
        }
        Name = trimmed;
        return Result.Success();
    }

    public Result DeActivate()
    {
        if (IsActive == false)
        {
            return Result.Failure(Errors.Artist.AlreadyInactive);
        }

        IsActive = false;
        return Result.Success();
    }
        
}
