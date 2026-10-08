using GigBooking.Application.Venues.Commands;
using GigBooking.Application.Venues.Commands.UpdateVenueCommand;
using GigBooking.Domain;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ReturnsExtensions;


namespace GigBooking.Application.UnitTests.Venues.Commands;

public class UpdateVenueCommandHandlerTests
{
    private static Location CreateValidLocation() => Location.Create("TestStreet", "TestPostalCode", "TestCity", "DK").Value;

    private readonly IVenueRepository _repositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly UpdateVenueCommandHandler _sut;

    public UpdateVenueCommandHandlerTests( ) { 
    
        
        _repositoryMock = Substitute.For<IVenueRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _sut = new UpdateVenueCommandHandler(_repositoryMock, _unitOfWorkMock);
    }
    [Fact]
    public async Task Handle_WithName_ReturnsSuccess()
    {
        var guid = Guid.NewGuid();
        var name = "ExistingName";
        var location = CreateValidLocation();
        Venue existingVenue = Venue.Create(new VenueId(guid), name,CreateValidLocation()).Value;
        _repositoryMock.GetByIdAsync(Arg.Any<VenueId>(), Arg.Any<CancellationToken>()).Returns(existingVenue);
        var newName = "NewName";
        var command = new UpdateVenueCommand(guid, newName, location);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(newName, existingVenue.Name);
        await _repositoryMock.Received(1).GetByIdAsync(Arg.Is<VenueId>(a => a.Value == guid), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

    }

    [Fact]
    public async Task Handle_NameMissing_ReturnsFailure()
    {
        var guid = Guid.NewGuid();
        var name = "ExistingName";
        var location = CreateValidLocation();
        Venue existingVenue = Venue.Create(new VenueId(guid), name, CreateValidLocation()).Value;
        _repositoryMock.GetByIdAsync(Arg.Any<VenueId>(), Arg.Any<CancellationToken>()).Returns(existingVenue);
        var newName = ""; // not valid name
        var command = new UpdateVenueCommand(guid, newName, location);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Venue.NoName, result.Error);
        await _repositoryMock.Received(1).GetByIdAsync(Arg.Is<VenueId>(a => a.Value == guid), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

    }

    [Fact]
    public async Task Handle_VenueIdNotFound_ReturnsFailure()
    {
        var guid = Guid.NewGuid();
        var name = "ExistingName";
        var location = CreateValidLocation();
        _repositoryMock.GetByIdAsync(Arg.Any<VenueId>(), Arg.Any<CancellationToken>()).ReturnsNull();
        var command = new UpdateVenueCommand(guid, name, location);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Venue.KeyNotFound, result.Error);
        await _repositoryMock.Received(1).GetByIdAsync(Arg.Is<VenueId>(a => a.Value == guid), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

    }

    [Fact]
    public async Task Handle_WhenLocationIsValid_ReturnsSuccess()
    {
        var guid = Guid.NewGuid();
        var name = "ExistingName";
        var location = CreateValidLocation();
        Venue existingVenue = Venue.Create(new VenueId(guid), name, location).Value;
        _repositoryMock.GetByIdAsync(Arg.Any<VenueId>(), Arg.Any<CancellationToken>()).Returns(existingVenue);
        Location newLocation = Location.Create("NewStreet", "NewPostalCode", "NewCity", "DK").Value;
        var command = new UpdateVenueCommand(guid, name, newLocation);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(newLocation, existingVenue.Location);
        await _repositoryMock.Received(1).GetByIdAsync(Arg.Is<VenueId>(a => a.Value == guid), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }


    [Fact]
    public async Task Handle_WhenLocationIsMissing_ReturnsFailure()
    {
        var guid = Guid.NewGuid();
        var name = "ExistingName";
        var location = CreateValidLocation();
        Venue existingVenue = Venue.Create(new VenueId(guid), name, CreateValidLocation()).Value;
        _repositoryMock.GetByIdAsync(Arg.Any<VenueId>(), Arg.Any<CancellationToken>()).Returns(existingVenue);
        Location? newLocation = null!;
        
        var command = new UpdateVenueCommand(guid, name, newLocation);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Venue.NullLocation, result.Error);
        await _repositoryMock.Received(1).GetByIdAsync(Arg.Is<VenueId>(a => a.Value == guid), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

    }
}
