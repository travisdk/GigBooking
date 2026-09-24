using GigBooking.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain;
using GigBooking.Domain.Entities;
using GigBooking.Application.Venues.Queries.GetVenue;

namespace GigBooking.Application.UnitTests.Venues.Queries;
public class GetVenueQueryHandlerTests
{

    private readonly IVenueRepository _venueRepositoryMock;
    private readonly GetVenueQueryHandler _sut;

    public GetVenueQueryHandlerTests()
    {
        _venueRepositoryMock = Substitute.For<IVenueRepository>();
        _sut = new GetVenueQueryHandler(_venueRepositoryMock);
    }


    [Fact]
    public async Task Handle_WhenArtistDoesExist_ShouldReturnArtist()
    {
        var rawId = Guid.NewGuid();
        var venueId = new VenueId(rawId);
        var query = new GetVenueQuery(rawId);
        var newVenue = Venue.Create(venueId, "TestVenue").Value;
        _venueRepositoryMock.GetByIdReadOnlyAsync(new VenueId(rawId), Arg.Any<CancellationToken>()).Returns(newVenue);

        var result = await _sut.Handle(query, CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(rawId, result.Value.Id);
        Assert.Equal(newVenue.Name, result.Value.Name);
    }


    [Fact]
    public async Task Handle_WhenArtistDoesNotExist_ShouldReturnNotFoundError()
    {
        var rawId = Guid.NewGuid();
        var query = new GetVenueQuery(rawId);
        _venueRepositoryMock.GetByIdAsync(new VenueId(rawId), Arg.Any<CancellationToken>()).ReturnsNull();

        var result = await _sut.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Venue.KeyNotFound, result.Error);

    }
}
