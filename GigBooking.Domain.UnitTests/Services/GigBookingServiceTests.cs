using GigBooking.Domain.Entities;
using GigBooking.Domain.Services;
using GigBooking.Domain.ValueObjects;

namespace GigBooking.Domain.UnitTests.Services;

public class GigBookingServiceTests
{
    [Fact]
    public void CanBook_WhenNoExistingGigs_ShouldReturnSuccess()
    {
       var gigService = new GigBookingService();
       var existingGigs = new List<Gig>(); // Empty
       var artistId = new ArtistId(Guid.NewGuid());
       var venueId =  new VenueId(Guid.NewGuid());
       var result = gigService.CanBook(artistId, venueId, existingGigs);
       Assert.True(result.IsSuccess);
    }

    [Fact]
    public void CanBook_WhenOtherGigsExistsWithoutConflict_ShouldReturnSuccess()
    {
        var gigService = new GigBookingService();
        var existingGig = Gig.Create(new GigId(Guid.NewGuid()), new ArtistId(Guid.NewGuid()), new VenueId(Guid.NewGuid()),
             DateTimeOffset.UtcNow.AddYears(1), DateTimeOffset.UtcNow).Value;
        var existingGigs = new List<Gig> { existingGig };
        var artistId = new ArtistId(Guid.NewGuid());
        var venueId = new VenueId(Guid.NewGuid());
        var result = gigService.CanBook(artistId, venueId, existingGigs);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void CanBook_WhenArtistAlreadyBooked_ShouldReturnFailure()
    {
        var gigService = new GigBookingService();
        var artistId = new ArtistId(Guid.NewGuid());
        var date = DateTimeOffset.UtcNow.AddYears(1);
        var existingGig = Gig.Create(new GigId(Guid.NewGuid()),artistId, new VenueId(Guid.NewGuid()),
             date, DateTimeOffset.UtcNow).Value;
        var existingGigs = new List<Gig> { existingGig };
        var result = gigService.CanBook(artistId, new VenueId(Guid.NewGuid()), existingGigs);
        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Gig.ArtistAlreadyBooked, result.Error);

    }

    [Fact]
    public void CanBook_WhenVenueAlreadyBooked_ShouldReturnFailure()
    {
        var gigService = new GigBookingService();
        var venueId = new VenueId(Guid.NewGuid());
        var date = DateTimeOffset.UtcNow.AddYears(1);
        var existingGig = Gig.Create(new GigId(Guid.NewGuid()), new ArtistId(Guid.NewGuid()), venueId, date,
              DateTimeOffset.UtcNow)
            .Value;
        var existingGigs = new List<Gig> { existingGig };
        var result = gigService.CanBook(new ArtistId(Guid.NewGuid()),venueId, existingGigs);
        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Gig.VenueAlreadyBooked, result.Error);
    }
}
