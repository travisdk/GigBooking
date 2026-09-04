using NSubstitute;
using NSubstitute.ReturnsExtensions;

using GigBooking.Application.Venues.Queries.GetVenue;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;
using GigBooking.Application.Venues.Queries.GetVenues;


namespace GigBooking.Application.UnitTests.Venues.Queries;

public class GetVenuesQueryHandlerTests
{
    private readonly IVenueRepository _venueRepositoryMock;
    private readonly GetVenuesQueryHandler _sut;

    public GetVenuesQueryHandlerTests()
    {
        _venueRepositoryMock = Substitute.For<IVenueRepository>();
        _sut = new GetVenuesQueryHandler(_venueRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnVenues()
    {
        var venue1 = Venue.Create(new VenueId(Guid.NewGuid()), "Test Venue 1").Value;
        var venue2 = Venue.Create(new VenueId(Guid.NewGuid()), "Test Venue 2").Value;
        List<Venue> existingVenues = [venue1, venue2];
        _venueRepositoryMock.GetAllVenuesAsync(Arg.Any<CancellationToken>()).Returns(existingVenues);
        var query = new GetVenuesQuery();
        var result = await _sut.Handle(query, CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
        Assert.Single(result.Value, a => a.Id == venue1.Id.Value && a.Name == venue1.Name); 
        Assert.Single(result.Value, a => a.Id == venue2.Id.Value && a.Name == venue2.Name); 
    }

}
