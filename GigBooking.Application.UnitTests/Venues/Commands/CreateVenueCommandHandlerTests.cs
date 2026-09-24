using GigBooking.Application.Common.Interfaces;
using GigBooking.Application.Venues.Commands.CreateVenueCommand;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Entities;
using GigBooking.Domain;
using NSubstitute;

namespace GigBooking.Application.UnitTests.Venues.Commands;

public class CreateVenueCommandHandlerTests
{

    private readonly IVenueRepository _venueRepositoryMock;
    private readonly CreateVenueCommandHandler _sut;
    private readonly IGuidGenerator _guidGeneratorMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    public CreateVenueCommandHandlerTests() 
    
    {
            _venueRepositoryMock = Substitute.For<IVenueRepository>();
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();
            _guidGeneratorMock = Substitute.For<IGuidGenerator>();
        _sut = new CreateVenueCommandHandler(_venueRepositoryMock, _unitOfWorkMock,
            _guidGeneratorMock);
      
   
    }
    [Fact]
    public async Task Handle_WithName_ReturnsNewVenue()
    {
        var guid = Guid.NewGuid();
        _guidGeneratorMock.GenerateGuid().Returns(guid);
        var command = new CreateVenueCommand("Wembley");

        var result = await _sut.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(guid, result.Value.Id);
        Assert.Equal("Wembley", result.Value.Name );
        
        _venueRepositoryMock.Received(1).Add(Arg.Any<Venue>());
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

    }


    [Fact]
    public async Task Handle_NameMissing_ReturnsFailure()
    {
        var command = new CreateVenueCommand(string.Empty);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Venue.NoName, result.Error);
        _venueRepositoryMock.DidNotReceive().Add(Arg.Any<Venue>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

    }

}
