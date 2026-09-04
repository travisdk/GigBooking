using GigBooking.Domain.Entities;
using GigBooking.Domain.Errors;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Domain.Services;

public class GigBookingService : IGigBookingService
{
    public Result CanBook(ArtistId artistId, VenueId venueId, IEnumerable<Gig> existingGigsOnDate)
    {
        foreach (Gig g in existingGigsOnDate)
        {

            if (g.ArtistId == artistId )
            {
                return Result.Failure(DomainErrors.Gig.ArtistAlreadyBooked);

            }
            if (g.VenueId == venueId)
            {
                return Result.Failure(DomainErrors.Gig.VenueAlreadyBooked);
            }
        }
        return Result.Success();
        
    }


}
