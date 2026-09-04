using GigBooking.Domain.Errors;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.ValueObjects;

namespace GigBooking.Domain.Entities;

public sealed class Gig : Entity<GigId>
{
    public ArtistId ArtistId { get; private set; }
    public VenueId VenueId { get; private set; }
    public DateTimeOffset ScheduledAt { get; private set; }

    private Gig(GigId id, ArtistId artistId, VenueId venueId, DateTimeOffset scheduledAt 
        )  : base(id)
    {
        this.ArtistId = artistId;
        this.VenueId = venueId; 
        this.ScheduledAt = scheduledAt;
    }
    public static Result<Gig> Create(GigId id, ArtistId artistId, VenueId venueId, 
          DateTimeOffset scheduledAt, DateTimeOffset utcNow)
    {
        if (scheduledAt < utcNow)
        {
            return Result<Gig>.Failure(DomainErrors.Gig.InvalidScheduledAtDate);
        }
        return Result<Gig>.Success(new Gig(id, artistId, venueId, scheduledAt));
    }
}
