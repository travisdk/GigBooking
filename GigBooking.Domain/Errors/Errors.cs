using GigBooking.Domain.Primitives;

namespace GigBooking.Domain;

public static class Errors
{
    public static class Location
    {
        public static readonly Error InvalidFields = new Error("Location.InvalidFields", "The Location contains one or more invalid fields");
        public static readonly Error InvalidCountryCode = new Error("Location.InvalidCountryCode", "The Country Code is not valid");
    }

    public static class Artist
    {
        public static readonly Error NoName = new Error("Artist.NoName", "The artist name cannot be empty or null.");
        public static readonly Error KeyNotFound = new Error("Artist.KeyNotFound", "The artist with the specified key was not found.");
        public static readonly Error EmptyArtistId = new Error("Artist.EmptyArtistId", "Artist identifier cannot be empty");
        public static readonly Error AlreadyInactive = new Error("Artist.AlreadyInactive", "Artist is already inactive");
    }
    public static class  Venue
    {
        public static readonly Error NoName = new Error("Venue.NoName", "The venue name cannot be empty or null.");
        public static readonly Error KeyNotFound = new Error("Venue.KeyNotFound", "The venue with the specified key was not found.");
        public static readonly Error EmptyVenueId = new Error("Venue.EmptyVenueId", "Venue identifier cannot be empty");
        public static readonly Error NullLocation = new Error("Venue.NullLocation", "Location cannot be null");
        public static readonly Error AlreadyInactive = new Error("Venue.AlreadyInactive", "Venue is already inactive");

    }
    public static class Gig
    {
        public static readonly Error CouldNotCreate = new Error("Gig.CouldNotCreate", "The new gig could not be created");
        public static readonly Error KeyNotFound = new Error("Gig.KeyNotFound", "The gig with the specified key was not found.");
        public static readonly Error ArtistAlreadyBooked = new Error("Gig.ArtistAlreadyBooked", "The artist is already booked on the specific date");
        public static readonly Error VenueAlreadyBooked = new Error("Gig.VenueAlreadyBooked", "The venue is already booked on the specific date");
        public static readonly Error InvalidScheduledAtDate = new Error("Gig.InvalidScheduledAtDate", "Scheduled date and time cannot be in the past.");
        public static readonly Error EmptyArtistId = new Error("Gig.EmptyArtistId", "Artist identifier cannot be empty");
        public static readonly Error EmptyVenueId =
            new("Gig.EmptyVenueId", "Venue identifier cannot be empty.");
        public static readonly Error EmptyGigId =
            new("Gig.EmptyGigId", "Gig identifier cannot be empty.");
    }
}
