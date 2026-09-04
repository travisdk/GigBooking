
using NSubstitute;

using GigBooking.Application.Gigs.Commands;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Services;
using GigBooking.Application.Common.Interfaces;
using Microsoft.Extensions.Time.Testing;
using GigBooking.Application.Artists.Commands.CreateArtistCommand;
using GigBooking.Domain.ValueObjects;
using GigBooking.Domain.Entities;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.Errors;
using NSubstitute.ReturnsExtensions;

namespace GigBooking.Application.UnitTests.Gigs.Commands;

public class CreateGigCommandHandlerTests
{
    private readonly CreateGigCommandHandler _sut;
    private readonly IGigRepository _gigRepositoryMock;
    private readonly IArtistRepository _artistRepositoryMock;
    private readonly IVenueRepository _venueRepositoryMock;
    private readonly IGigBookingService _gigBookingServiceMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IGuidGenerator _guidGeneratorMock;
    private readonly FakeTimeProvider _timeProviderMock;
    private DateTimeOffset _fixedTime;
    public CreateGigCommandHandlerTests() {

        _gigRepositoryMock = Substitute.For<IGigRepository>();
        _artistRepositoryMock = Substitute.For<IArtistRepository>();
        _venueRepositoryMock = Substitute.For<IVenueRepository>();
        _gigBookingServiceMock = Substitute.For<IGigBookingService>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _guidGeneratorMock = Substitute.For<IGuidGenerator>();

        _fixedTime = new DateTimeOffset(2026, 6, 1, 12, 0, 0, TimeSpan.Zero);
        _timeProviderMock  = new FakeTimeProvider(_fixedTime);
        
        _sut = new CreateGigCommandHandler(_gigRepositoryMock, _artistRepositoryMock, _venueRepositoryMock
             , _gigBookingServiceMock, _unitOfWorkMock, _guidGeneratorMock, _timeProviderMock );
    
    }

    [Fact]
    public async Task Handle_WhenScheduledAtIsCorrect_ReturnsNewGig() {
        var artistGuid = Guid.NewGuid();
        var venueGuid  = Guid.NewGuid();
        var expectedGigGuid = Guid.NewGuid();
        var artist = Artist.Create(new ArtistId(artistGuid), "Artist").Value;
        _artistRepositoryMock.GetByIdReadOnlyAsync(Arg.Any<ArtistId>(), Arg.Any<CancellationToken>()).
            Returns(artist);
        var venue = Venue.Create(new VenueId(venueGuid), "Venue").Value;
        _venueRepositoryMock.GetByIdReadOnlyAsync(Arg.Any<VenueId>(), Arg.Any<CancellationToken>()).
              Returns(venue);
        _gigBookingServiceMock.CanBook(artist.Id, venue.Id, Arg.Any<IEnumerable<Gig>>() )
             .Returns(Result.Success());
        _guidGeneratorMock.GenerateGuid().Returns(expectedGigGuid);
        var command = new CreateGigCommand(artistGuid, venueGuid,
              _timeProviderMock.GetUtcNow().AddDays(7) );

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess );
        Assert.Equal(expectedGigGuid, result.Value.Id);
        _gigRepositoryMock.Received(1).Add(Arg.Is<Gig>(g => g.Id.Value == expectedGigGuid));
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenScheduledAtIsInPast_ReturnsFailure()
    {
        var artistGuid = Guid.NewGuid();
        var venueGuid = Guid.NewGuid();
        var expectedGigGuid = Guid.NewGuid();
        var artist = Artist.Create(new ArtistId(artistGuid), "Artist").Value;
        _artistRepositoryMock.GetByIdReadOnlyAsync(Arg.Any<ArtistId>(), Arg.Any<CancellationToken>()).
            Returns(artist);
        var venue = Venue.Create(new VenueId(venueGuid), "Venue").Value;
        _venueRepositoryMock.GetByIdReadOnlyAsync(Arg.Any<VenueId>(), Arg.Any<CancellationToken>()).
              Returns(venue);
        _gigBookingServiceMock.CanBook(artist.Id, venue.Id, Arg.Any<IEnumerable<Gig>>())
             .Returns(Result.Success());
        _guidGeneratorMock.GenerateGuid().Returns(expectedGigGuid);
        var command = new CreateGigCommand(artistGuid, venueGuid,
              _timeProviderMock.GetUtcNow().AddDays(-7)); // PAST

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Gig.InvalidScheduledAtDate, result.Error);
         _gigRepositoryMock.DidNotReceive().Add(Arg.Any<Gig>());
         await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

    }
    [Fact]
    public async Task Handle_WhenArtistIsNotFound_ReturnsFailure()
    {
        
        _artistRepositoryMock.GetByIdReadOnlyAsync(Arg.Any<ArtistId>(), Arg.Any<CancellationToken>())
              .ReturnsNull(); // NSub default but for clarity.

        var command = new CreateGigCommand(Guid.NewGuid(), Guid.NewGuid(), _timeProviderMock.GetUtcNow().AddDays(7));

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Artist.KeyNotFound, result.Error);
        _gigRepositoryMock.DidNotReceive().Add(Arg.Any<Gig>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        
    }

    [Fact]
    public async Task Handle_WhenVenueIsNotFound_ReturnsFailure()
    {
        var artistGuid= Guid.NewGuid();
        var artist = Artist.Create(new ArtistId(artistGuid),
                  "TestArtist").Value;
        _artistRepositoryMock.GetByIdReadOnlyAsync(Arg.Is<ArtistId>( id => id.Value == artistGuid), Arg.Any<CancellationToken>())
            .Returns( artist 
            );
        _venueRepositoryMock.GetByIdReadOnlyAsync(Arg.Any<VenueId>(), Arg.Any<CancellationToken>())
             .ReturnsNull();
        var command = new CreateGigCommand(artistGuid, Guid.NewGuid(), _timeProviderMock.GetUtcNow().AddDays(7));

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Venue.KeyNotFound, result.Error);
        _gigRepositoryMock.DidNotReceive().Add(Arg.Any<Gig>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

    }

    [Fact]
    public async Task Handle_WhenCanBookIsNotAllowed_ReturnsFailure()
    {
        var artistGuid = Guid.NewGuid();
        var venueGuid = Guid.NewGuid();
        var artist = Artist.Create(new ArtistId(artistGuid),
                  "TestArtist").Value;
        var venue = Venue.Create(new VenueId(venueGuid), "TestVenue").Value;

        _artistRepositoryMock.GetByIdReadOnlyAsync(Arg.Is<ArtistId>(id => id.Value == artistGuid), Arg.Any<CancellationToken>())
            .Returns(artist
            );
        _venueRepositoryMock.GetByIdReadOnlyAsync(Arg.Is<VenueId>(id => id.Value == venueGuid), Arg.Any<CancellationToken>())
              .Returns(venue
            );


        _gigBookingServiceMock.CanBook(Arg.Is<ArtistId>(id => id.Value == artistGuid), Arg.Is<VenueId>(
              id => id.Value == venueGuid),
                Arg.Any<IEnumerable<Gig>>()
              ).Returns(Result.Failure(DomainErrors.Gig.ArtistAlreadyBooked));
        // ArtistAlreadyBooked is the first possible error

        var command = new CreateGigCommand(artistGuid, venueGuid, _timeProviderMock.GetUtcNow().AddDays(7));

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Gig.ArtistAlreadyBooked, result.Error);

        _gigRepositoryMock.DidNotReceive().Add(Arg.Any<Gig>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

    }
}
