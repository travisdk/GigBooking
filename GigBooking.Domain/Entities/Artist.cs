using GigBooking.Domain.Errors;
using GigBooking.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GigBooking.Domain.Entities;

public class Artist : Entity
{
    public string Name { get; private set; } = string.Empty;
    private Artist(Guid id, string name ) : base(id)
    {
        Name = name;
    } 

    public static Result<Artist> Create(Guid id, string name)
    {

        if (string.IsNullOrEmpty(name))
        {
            return Result<Artist>.Failure(DomainErrors.Artist.NoName);
        }
        return Result<Artist>.Success(new Artist(id, name));
    }
}
