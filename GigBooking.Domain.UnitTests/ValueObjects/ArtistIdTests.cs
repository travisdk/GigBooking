using GigBooking.Domain.ValueObjects;

namespace GigBooking.Domain.UnitTests.ValueObjects;

public class ArtistIdTests
{

    [Fact]
    public void Equals_WhenIdsAreIdentical_ShouldBeEqual()
    {
        var guid = Guid.NewGuid();
        var artistId1 = new ArtistId(guid);
        var artistId2 = new ArtistId(guid);
        Assert.Equal(artistId1, artistId2);
        Assert.True(artistId1 == artistId2);
    }
    [Fact]
    public void Equals_WhenIdsAreDifferent_ShouldNotBeEqual()
    {
        var artistId1 = new ArtistId(Guid.NewGuid());
        var artistId2 = new ArtistId(Guid.NewGuid());
        Assert.NotEqual(artistId1, artistId2);
        Assert.False(artistId1 == artistId2);
    }
}
