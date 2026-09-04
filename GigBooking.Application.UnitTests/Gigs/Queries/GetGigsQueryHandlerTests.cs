using NSubstitute;

using GigBooking.Application.Gigs.Queries.GetGigs;
using GigBooking.Application.Common.Interfaces;
using GigBooking.Application.Gigs.DTOs;

namespace GigBooking.Application.UnitTests.Gigs.Queries;
public class GetGigsQueryHandlerTests
{
    private readonly IGigReadRepository _gigRepositoryMock;
    private readonly GetGigsQueryHandler _sut;

    public GetGigsQueryHandlerTests()
    {
        _gigRepositoryMock = Substitute.For<IGigReadRepository>();
        _sut = new GetGigsQueryHandler(_gigRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnGigs()
    {
        List<GigDetailsDTO> existingGigs = new List<GigDetailsDTO>();
        var gig1Id = Guid.NewGuid();
        var gig2Id = Guid.NewGuid();
        var artist1Id = Guid.NewGuid();
        var artist2Id = Guid.NewGuid();
        var venue1Id = Guid.NewGuid();
        var venue2Id = Guid.NewGuid();
        var time1 = DateTimeOffset.Parse("2027-09-03T10:45:00+02:00");
        var time2 = DateTimeOffset.Parse("2027-05-03T10:45:00+02:00");
        existingGigs.Add(new GigDetailsDTO
        (
            gig1Id,
            artist1Id,
            venue1Id,
            time1,
            "John Lennon",
            "Wembley"
        ));
        existingGigs.Add(new GigDetailsDTO
        (gig2Id,
            artist2Id,
            venue2Id,
            time2,
            "The Who",
            "Mets Stadium"
        ));
        var query = new GetGigsQuery();
        _gigRepositoryMock.GetAllGigsWithDetailsAsync(Arg.Any<CancellationToken>()).Returns(existingGigs);

        var result = await _sut.Handle(query, CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
        Assert.Single(result.Value, g => g.Id == gig1Id && g.ArtistId == artist1Id && g.VenueId == venue1Id &&
            g.ScheduledAt == time1 && g.ArtistName == "John Lennon" && g.VenueName == "Wembley");
        Assert.Single(result.Value, g => g.Id == gig2Id && g.ArtistId==artist2Id && g.VenueId==venue2Id &&
            g.ScheduledAt == time2 && g.ArtistName == "The Who" && g.VenueName == "Mets Stadium");
    }
}
