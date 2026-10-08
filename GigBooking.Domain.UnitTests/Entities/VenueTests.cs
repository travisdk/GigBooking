using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;
using System.Xml.Linq;


namespace GigBooking.Domain.UnitTests.Entities;


public class VenueTests
{
    private static Location CreateValidLocation() => Location.Create("TestStreet", "TestPostalCode", "TestCity", "DK").Value;


    [Fact]
    public void Create_ValidParameters_ReturnsSuccess()
    {
        var venueId = new VenueId(Guid.NewGuid());
        var name = "TestVenue";
        var location = CreateValidLocation();
        
        var result = Venue.Create(venueId, name, location);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(venueId, result.Value.Id);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(location, result.Value.Location);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NameMissing_ReturnsFailure(string? name)
    {
        var venueId = new VenueId(Guid.NewGuid());
        var location = CreateValidLocation();
        var result = Venue.Create(venueId, name!, location);
        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Venue.NoName, result.Error);
    }


    [Fact]
    public void Create_LocationIsNull_ReturnsFailure()
    {
        var venueId = new VenueId(Guid.NewGuid());
        var result = Venue.Create(venueId, "TestVenue", null!);
        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Venue.NullLocation, result.Error);
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ReturnsFailure()
    {
        var location = CreateValidLocation();

        var venue = Venue.Create(new VenueId(Guid.NewGuid()), "TestVenue", location).Value;
        venue.DeActivate();

        var result = venue.DeActivate();

        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Venue.AlreadyInactive, result.Error);

    }
    [Fact]
    public void Deactivate_WhenActive_SetsIsActiveToFalseAndReturnsSuccess()
    {
        var location = CreateValidLocation();

        var venue = Venue.Create(new VenueId(Guid.NewGuid()), "TestVenue", location).Value;

        var result = venue.DeActivate();

        Assert.True(result.IsSuccess);
        Assert.False(venue.IsActive);
    }

    [Fact]
    public void ChangeName_NameIsApplied_ChangesNameAndReturnsSuccess()
    {
        var location = CreateValidLocation();
        var venue = Venue.Create(new VenueId(Guid.NewGuid()), "TestVenue", location).Value;

        var result = venue.ChangeName("NewName");

        Assert.True(result.IsSuccess);
        Assert.Equal("NewName", venue.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void ChangeName_NameMissing_ReturnsFailure(string? name)
    {
        var location = CreateValidLocation();
        var venue = Venue.Create(new VenueId(Guid.NewGuid()), "TestVenue", location).Value;
        
        var result = venue.ChangeName(name!);

        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Venue.NoName, result.Error);
        Assert.Equal("TestVenue", venue.Name);
    }

}