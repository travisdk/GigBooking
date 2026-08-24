using GigBooking.Domain.Primitives;

namespace GigBooking.Domain.Entities;

public sealed class Gig : Entity
{
    public Guid ArtistId { get; private set; }
    public Guid VenueId { get; private set; }
    public DateTime ScheduledAt { get; private set; }

    private Gig(Guid Id, Guid ArtistId, Guid VenueId, DateTime ScheduledAt)  : base(Id)
    {
        this.ArtistId = ArtistId;
        this.VenueId = VenueId;
        this.ScheduledAt = ScheduledAt;
    }
    public static Result<Gig> Create(Guid Id, Guid ArtistId, Guid VenueId, DateTime ScheduledAt)
    {
        if (ScheduledAt < DateTime.UtcNow)
        {
            return Result<Gig>.Failure(new Error("InvalidScheduledAt", "Scheduled date and time cannot be in the past."));
        }
        return Result<Gig>.Success(new Gig(Id, ArtistId, VenueId, ScheduledAt));
    }
}
