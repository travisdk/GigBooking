using NSubstitute;
using NSubstitute.ReturnsExtensions;
using GigBooking.Domain.ValueObjects;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain;
using GigBooking.Domain.Entities;
using GigBooking.Application.Gigs.Queries.GetGig;


namespace GigBooking.Application.UnitTests.Gigs.Queries;
public class GetGigQueryHandlerTests
{
    private readonly IGigRepository _gigRepositoryMock;
    private readonly GetGigQueryHandler _sut;

    public GetGigQueryHandlerTests()
    {
        _gigRepositoryMock = Substitute.For<IGigRepository>();
        _sut = new GetGigQueryHandler(_gigRepositoryMock);
    }
    [Fact]
    public async Task Handle_WhenGigDoesExist_ShouldReturnGig()
    {
        var rawId = Guid.NewGuid();
        var query = new GetGigQuery(rawId);
        var gigId = new GigId(rawId);
        var artistId = new ArtistId(Guid.NewGuid());
        var venueId = new VenueId(Guid.NewGuid());
        var utcNow = DateTimeOffset.UtcNow;
        var scheduledAt = utcNow.AddDays(1);
        var newGig = Gig.Create(gigId, artistId, venueId, scheduledAt, utcNow).Value;
         _gigRepositoryMock.GetByIdReadOnlyAsync(gigId, Arg.Any<CancellationToken>()).Returns(newGig);

        var result = await _sut.Handle(query, CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(rawId, result.Value.Id);
        Assert.Equal(artistId.Value, result.Value.ArtistId);
        Assert.Equal(venueId.Value, result.Value.VenueId);
        Assert.Equal(scheduledAt, result.Value.ScheduledAt);
    }

    [Fact]
    public async Task Handle_WhenGigDoesNotExist_ShouldReturnNotFoundError()
    {
        var rawId = Guid.NewGuid();
        var query = new GetGigQuery(rawId);
        _gigRepositoryMock.GetByIdReadOnlyAsync(new GigId(rawId), Arg.Any<CancellationToken>()).ReturnsNull();

        var result = await _sut.Handle(query, CancellationToken.None);
        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Gig.KeyNotFound, result.Error);

    }

}
