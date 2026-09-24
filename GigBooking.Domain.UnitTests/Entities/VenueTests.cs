using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;


namespace GigBooking.Domain.UnitTests.Entities;

public class VenueTests
{
    [Fact]
    public void Create_NameApplied_ReturnsSucces()
    {
        var venueId = new VenueId(Guid.NewGuid());
        var name = "TestVenue";
        var result = Venue.Create(venueId, name);
        Assert.True(result.IsSuccess);
        Assert.Equal(venueId, result.Value.Id);
        Assert.Equal(name, result.Value.Name);
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NameMissing_ReturnsFailure(string? name)
    {
        var venueId = new VenueId(Guid.NewGuid());
        var result = Venue.Create(venueId, name!);
        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Venue.NoName, result.Error);
    }


}