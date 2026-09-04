using GigBooking.Domain.Entities;
using GigBooking.Domain.Errors;
using GigBooking.Domain.ValueObjects;


namespace GigBooking.Domain.UnitTests.Entities;

public class GigTests
{

    [Fact]
    public void Create_InvalidScheduledAtDate_ReturnsFailure()
    {
        var utcNow  = DateTimeOffset.UtcNow;
        var invalidDateTime = utcNow.AddDays(-1); // past
        
        var result = Gig.Create(
            new GigId(Guid.NewGuid()), 
            new ArtistId(Guid.NewGuid()),
            new VenueId(Guid.NewGuid()),
            invalidDateTime, utcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Gig.InvalidScheduledAtDate, result.Error);

    }

}
