using GigBooking.Application.Artists.Commands.CreateArtistCommand;
using GigBooking.Application.Common.Interfaces;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Entities;
using GigBooking.Domain.Errors;
using NSubstitute;

namespace GigBooking.Application.UnitTests.Artists.Commands;

public class CreateArtistCommandHandlerTests
{
    private readonly IArtistRepository _artistRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IGuidGenerator _guidGeneratorMock;
    private readonly CreateArtistCommandHandler _sut;
    public CreateArtistCommandHandlerTests() 
    
    {
            
        _artistRepositoryMock = Substitute.For<IArtistRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _guidGeneratorMock = Substitute.For<IGuidGenerator>();
        _sut = new CreateArtistCommandHandler(_artistRepositoryMock , 
               _unitOfWorkMock, 
               _guidGeneratorMock);
    
    }
    [Fact]
    public async Task Handle_WithName_ReturnsNewArtist()
    {
        var expectedGuid = Guid.NewGuid();
        _guidGeneratorMock.GenerateGuid().Returns(expectedGuid);
        var command = new CreateArtistCommand("Metallica");

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(command.Name, result.Value.Name);
        Assert.Equal(expectedGuid, result.Value.Id);

       _artistRepositoryMock.Received(1).Add(Arg.Any<Artist>());
        await _unitOfWorkMock.Received(1).
            SaveChangesAsync(Arg.Any<CancellationToken>());

    }
    

    [Fact]
    public async Task Handle_NameMissing_ReturnsFailure()
    {
        var command = new CreateArtistCommand(string.Empty);

        var result = await _sut.Handle(command, CancellationToken.None);
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Artist.NoName, result.Error);

        _artistRepositoryMock.DidNotReceive().Add(Arg.Any<Artist>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

    }

}
