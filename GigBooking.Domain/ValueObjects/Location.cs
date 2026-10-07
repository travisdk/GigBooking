using GigBooking.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using GigBooking.Domain;

namespace GigBooking.Domain.ValueObjects;

public sealed record Location
{
    public string Street { get; }
    public string PostalCode { get;  }
    public string City { get; } 
    public string CountryCode { get;  }  // I.e. DK, SE, NO... ISO 3166-1 Alpha-2

    private Location(string street, string postalCode, string city, string countryCode)
    {
        Street = street;
        PostalCode = postalCode;
        City = city;
        CountryCode = countryCode.ToUpperInvariant();
    }
    public static Result<Location> Create(string street, string postalCode, string city, string countryCode)
    {
        if (string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(postalCode) || string.IsNullOrWhiteSpace(city)
              )
        {
            return Result<Location>.Failure(Errors.Location.InvalidFields);
        }

        if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Trim().Length != 2)
        {
            return Result<Location>.Failure(Errors.Location.InvalidCountryCode);
        }

        return Result<Location>.Success(new Location(street, postalCode, city, countryCode));
    }

}
