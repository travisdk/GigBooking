using GigBooking.Application.Artists.Queries.GetArtist;
using GigBooking.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Errors;
using Microsoft.VisualBasic;
using GigBooking.Domain.Entities;

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
        var newArtist = Artist.Create(artistId, "TestArtist").Value;
        _artistRepositoryMock.GetByIdReadOnlyAsync(new ArtistId(rawId), Arg.Any<CancellationToken>()).Returns(newArtist);

        var result = await _sut.Handle(query, CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(rawId, result.Value.Id);
        Assert.Equal(newArtist.Name, result.Value.Name);
    }

    [Fact]
    public async Task Handle_WhenArtistDoesNotExist_ShouldReturnNotFoundError()
    {
        var rawId = Guid.NewGuid();
        var query = new GetArtistQuery(rawId);
        _artistRepositoryMock.GetByIdAsync(new ArtistId(rawId), Arg.Any<CancellationToken>()).ReturnsNull();

        var result = await _sut.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Artist.KeyNotFound, result.Error);

    }
}
