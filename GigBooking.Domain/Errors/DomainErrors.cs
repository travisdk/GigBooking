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
    public static class  Venue
    {
        public static Error NoName => new Error("Venue.NoName", "The venue name cannot be empty or null.");
        public static Error KeyNotFound => new Error("Venue.KeyNotFound", "The venue with the specified key was not found.");


    }
    public static class Gig
    {
        public static Error CouldNotCreate => new Error("Gig.CouldNotCreate", "The new gig could not be created");
        public static Error KeyNotFound => new Error("Gig.KeyNotFound", "The gig with the specified key was not found.");
        public static Error ArtistAlreadyBooked = new Error("Gig.ArtistAlreadyBooked", "The artist is already booked on the specific date");
        public static Error VenueAlreadyBooked = new Error("Gig.VenueAlreadyBooked", "The venue is already booked on the specific date");
        public static Error InvalidScheduledAtDate = new Error("Gig.InvalidScheduledAtDate", "Scheduled date and time cannot be in the past.");
    }
}
