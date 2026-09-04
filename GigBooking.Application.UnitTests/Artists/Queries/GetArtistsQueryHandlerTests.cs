using NSubstitute;
using GigBooking.Domain.ValueObjects;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Entities;
using GigBooking.Application.Artists.Queries.GetArtists;

namespace GigBooking.Application.UnitTests.Artists.Queries;
public class GetArtistsQueryHandlerTests
{

    private readonly IArtistRepository _artistRepositoryMock;
    private readonly GetArtistsQueryHandler _sut;

    public GetArtistsQueryHandlerTests()
    {
        _artistRepositoryMock = Substitute.For<IArtistRepository>();
        _sut = new GetArtistsQueryHandler(_artistRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnArtists()
    {
        var artist1 = Artist.Create(new ArtistId(Guid.NewGuid()), "Artist First").Value;
        var artist2 = Artist.Create(new ArtistId(Guid.NewGuid()), "Artist Second").Value;
        List<Artist> existingArtists = [artist1, artist2];
        _artistRepositoryMock.GetAllArtistsAsync(Arg.Any<CancellationToken>()).Returns(existingArtists);
        var query = new GetArtistsQuery();
        
        var result = await _sut.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);

        Assert.Single(result.Value, a => a.Id == artist1.Id.Value && a.Name == artist1.Name);
        Assert.Single(result.Value, a => a.Id == artist2.Id.Value && a.Name == artist2.Name);
    }
   
}
