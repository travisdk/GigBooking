using GigBooking.Domain.ValueObjects;

namespace GigBooking.Domain.UnitTests.ValueObjects;

public class GigIdTests
{

    [Fact]
    public void Equals_WhenIdsAreIdentical_ShouldBeEqual()
    {
        var guid = Guid.NewGuid();
        var gigId1 = new GigId(guid);
        var gigId2 = new GigId(guid);
        Assert.Equal(gigId1, gigId2);
        Assert.True(gigId1 == gigId2);
    }
    [Fact]
    public void Equals_WhenIdsAreDifferent_ShouldNotBeEqual()
    {
        var gigId1 = new GigId(Guid.NewGuid());
        var gigId2 = new GigId(Guid.NewGuid());
        Assert.NotEqual(gigId1, gigId2);
        Assert.False(gigId1 == gigId2);
    }
}
