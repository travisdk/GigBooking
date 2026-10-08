using GigBooking.Domain;
using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;
using System.Xml.Linq;



namespace GigBooking.Domain.UnitTests.Entities;

public class ArtistTests
{
    [Fact]
    public void Create_NameIsApplied_ReturnsSuccess()
    {
        var artistId = new ArtistId(Guid.NewGuid());
        var artistName = "TestArtist";
        var result = Artist.Create(artistId, artistName);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(artistId, result.Value.Id);
        Assert.Equal(artistName, result.Value.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void Create_NameMissing_ReturnsFailure(string? name)
    {
        var result = Artist.Create(new ArtistId(Guid.NewGuid()), name!);
        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Artist.NoName, result.Error);
    }


    [Fact]
    public void Deactivate_WhenAlreadyInactive_ReturnsFailure()
    {
        var artist = Artist.Create(new ArtistId(Guid.NewGuid()), "TestArtist").Value;
        artist.DeActivate();
        
        var result = artist.DeActivate();
        
        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Artist.AlreadyInactive, result.Error);
        
    }
    [Fact] 
    public void Deactivate_WhenActive_SetsIsActiveToFalseAndReturnsSuccess()
    {
        var artist = Artist.Create(new ArtistId(Guid.NewGuid()), "TestArtist").Value;
        
        var result = artist.DeActivate();
        
        Assert.True(result.IsSuccess);
        Assert.False(artist.IsActive);
    }

    [Fact]
    public void ChangeName_NameIsApplied_ChangesNameAndReturnsSuccess()
    {
        var artist = Artist.Create(new ArtistId(Guid.NewGuid()), "TestArtist").Value;

        var result = artist.ChangeName("NewName");

        Assert.True(result.IsSuccess);
        Assert.Equal("NewName", artist.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void ChangeName_NameMissing_ReturnsFailure(string? name)
    {
        var artist = Artist.Create(new ArtistId(Guid.NewGuid()), "TestArtist").Value;

        var result = artist.ChangeName(name!);

        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Artist.NoName, result.Error);
        Assert.Equal("TestArtist", artist.Name);
    }
}
