using GigBooking.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Application.Venues.DTOs;

public sealed record LocationDTO(string Street, string Postalcode, string City, string CountryCode)
{
    public static LocationDTO FromDomain(Location location) =>
        new(location.Street, location.PostalCode, location.City, location.CountryCode);
}

