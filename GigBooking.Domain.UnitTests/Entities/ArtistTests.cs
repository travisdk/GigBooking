using GigBooking.Domain.Entities;
using GigBooking.Domain.Errors;
using GigBooking.Domain.ValueObjects;


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
        Assert.Equal(DomainErrors.Artist.NoName, result.Error);
    }

}
