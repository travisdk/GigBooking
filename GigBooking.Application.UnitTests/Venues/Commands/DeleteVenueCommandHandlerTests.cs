using GigBooking.Application.Venues.Commands;
using GigBooking.Application.Venues.Commands.DeleteVenueCommand;
using GigBooking.Domain;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ReturnsExtensions;



namespace GigBooking.Application.UnitTests.Venues.Commands;

public class DeleteVenueCommandHandlerTests
{
    private readonly IVenueRepository _repositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly DeleteVenueCommandHandler _sut;
    private static Location CreateValidLocation() => Location.Create("TestStreet", "TestPostalCode", "TestCity", "DK").Value;

    public DeleteVenueCommandHandlerTests()
    {


        _repositoryMock = Substitute.For<IVenueRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _sut = new DeleteVenueCommandHandler(_repositoryMock, _unitOfWorkMock);
    }
    [Fact]
    public async Task Handle_VenueIsFound_DeactivatesAndReturnsSuccess()
    {
        var guid = Guid.NewGuid();
        var venue = Venue.Create(new VenueId(guid), "TestArtist", CreateValidLocation() ).Value;
         _repositoryMock.GetByIdAsync(Arg.Is<VenueId>(a => a.Value == guid), Arg.Any<CancellationToken>()).Returns(venue);
        var command = new DeleteVenueCommand(guid);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(venue.IsActive);
        await _repositoryMock.Received(1).GetByIdAsync(Arg.Is<VenueId>(v => v.Value == guid), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    [Fact]
    public async Task Handle_VenueIsNotFound_ReturnsFailure()
    {
        var guid = Guid.NewGuid();
        _repositoryMock.GetByIdAsync(Arg.Any<VenueId>(), Arg.Any<CancellationToken>()).ReturnsNull();
        var command = new DeleteVenueCommand(guid);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Venue.KeyNotFound, result.Error);

        await _repositoryMock.Received(1).GetByIdAsync(Arg.Is<VenueId>(v => v.Value == guid), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

    }

    [Fact]
    public async Task Handle_DeactivateAlreadyInactive_ReturnsFailure()
    {
        var guid = Guid.NewGuid();
        var venue = Venue.Create(new VenueId(guid), "TestVenue", CreateValidLocation()).Value;
        venue.DeActivate();
        _repositoryMock.GetByIdAsync(Arg.Is<VenueId>(v => v.Value == guid), Arg.Any<CancellationToken>()).Returns(venue);
        var command = new DeleteVenueCommand(guid);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Venue.AlreadyInactive, result.Error);
        await _repositoryMock.Received(1).GetByIdAsync(Arg.Is<VenueId>(v => v.Value == guid), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

    }
}
