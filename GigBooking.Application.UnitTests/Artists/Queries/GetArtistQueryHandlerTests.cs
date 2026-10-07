
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain;
using GigBooking.Domain.Entities;
using GigBooking.Application.Artists.Queries.GetArtist;
using GigBooking.Domain.ValueObjects;


namespace GigBooking.Application.UnitTests.Artists.Queries;
public class GetArtistQueryHandlerTests
{

    private readonly IArtistRepository _artistRepositoryMock;
    private readonly GetArtistQueryHandler _sut;

    public GetArtistQueryHandlerTests()
    {
        _artistRepositoryMock = Substitute.For<IArtistRepository>();
        _sut = new GetArtistQueryHandler(_artistRepositoryMock);
    }
    [Fact]
    public async Task Handle_WhenArtistDoesExist_ShouldReturnArtist()
    {
        var rawId = Guid.NewGuid();
        var artistId = new ArtistId(rawId);
        var query = new GetArtistQuery(rawId);
        var artist = Artist.Create(artistId, "TestArtist").Value;
        artist.DeActivate(); // to test inactive status is also considered here
        _artistRepositoryMock.GetByIdReadOnlyAsync(new ArtistId(rawId), Arg.Any<CancellationToken>()).Returns(artist);

        var result = await _sut.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(rawId, result.Value.Id);
        Assert.Equal(artist.Name, result.Value.Name);
        Assert.False(artist.IsActive);
    }

    [Fact]
    public async Task Handle_WhenArtistDoesNotExist_ShouldReturnNotFoundError()
    {
        var rawId = Guid.NewGuid();
        var query = new GetArtistQuery(rawId);
        _artistRepositoryMock.GetByIdAsync(new ArtistId(rawId), Arg.Any<CancellationToken>()).ReturnsNull();

        var result = await _sut.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Artist.KeyNotFound, result.Error);

    }
}
