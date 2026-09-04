using GigBooking.Domain.Entities;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Domain.Services;

public interface IGigBookingService
{

    public Result CanBook(ArtistId artistId, VenueId venueId, IEnumerable<Gig> existingGigsOnDate);

}
