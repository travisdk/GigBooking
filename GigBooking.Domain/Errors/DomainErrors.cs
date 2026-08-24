using System;
using System.Collections.Generic;
using System.Text;
using GigBooking.Domain.Primitives;

namespace GigBooking.Domain.Errors;

public static class DomainErrors
{
    public static class Artist
    {
        public static Error NoName => new Error("Artist.NoName", "The artist name cannot be empty or null.");
        public static Error KeyNotFound => new Error("Artist.KeyNotFound", "The artist with the specified key was not found.");

    }
}
