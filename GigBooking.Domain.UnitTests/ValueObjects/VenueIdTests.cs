using GigBooking.Domain.ValueObjects;

namespace GigBooking.Domain.UnitTests.ValueObjects;

public class VenueIdTests
{

    [Fact]
    public void Equals_WhenIdsAreIdentical_ShouldBeEqual()
    {
        var guid = Guid.NewGuid();
        var venueId1 = new VenueId(guid);
        var venueId2 = new VenueId(guid);
        Assert.Equal(venueId1, venueId2);
        Assert.True(venueId1 == venueId2);
    }
    [Fact]
    public void Equals_WhenIdsAreDifferent_ShouldNotBeEqual()
    {
        var venueId1 = new VenueId(Guid.NewGuid());
        var venueId2 = new VenueId(Guid.NewGuid());
        Assert.NotEqual(venueId1, venueId2);
        Assert.False(venueId1 == venueId2);
    }
}
