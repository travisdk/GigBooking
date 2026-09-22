using GigBooking.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Application.Common.Errors;

public static class ValidationErrors
{

    public static class Gig
    {
        public static readonly Error EmptyArtistId = new Error("Validation.Gig.EmptyArtistId", "Artist identifier cannot be empty");
        public static readonly Error EmptyVenueId =
            new("Validation.Gig.EmptyVenueId", "Venue identifier cannot be empty.");


    }
}
