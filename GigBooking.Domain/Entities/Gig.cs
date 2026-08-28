using GigBooking.Domain.Primitives;
using GigBooking.Domain.ValueObjects;

namespace GigBooking.Domain.Entities;

public sealed class Gig : Entity<GigId>
{
    public ArtistId ArtistId { get; private set; }
    public VenueId VenueId { get; private set; }
    public DateTime ScheduledAt { get; private set; }

    private Gig(GigId id, ArtistId artistId, VenueId venueId, DateTime scheduledAt)  : base(id)
    {
        this.ArtistId = artistId;
        this.VenueId = venueId; 
        this.ScheduledAt = scheduledAt;
    }
    public static Result<Gig> Create(GigId id, ArtistId artistId, VenueId venueId, DateTime scheduledAt)
    {
        if (scheduledAt < DateTime.UtcNow)
        {
            return Result<Gig>.Failure(new Error("InvalidScheduledAt", "Scheduled date and time cannot be in the past."));
        }
        return Result<Gig>.Success(new Gig(id, artistId, venueId, scheduledAt));
    }
}
