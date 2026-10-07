using GigBooking.Application.Artists.Commands;
using GigBooking.Application.Artists.Commands.CreateArtistCommand;
using GigBooking.Domain;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace GigBooking.Application.UnitTests.Artists.Commands;

public class DeleleteArtistCommandHandlerTests
{
    private readonly IArtistRepository _repositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly DeleteArtistCommandHandler _sut;

    public DeleleteArtistCommandHandlerTests()
    {


        _repositoryMock = Substitute.For<IArtistRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _sut = new DeleteArtistCommandHandler(_repositoryMock, _unitOfWorkMock);
    }
    [Fact]
    public async Task Handle_ArtistIsFound_DeactivatesAndReturnsSuccess()
    {
        var guid = Guid.NewGuid();
        var artist = Artist.Create(new ArtistId(guid), "TestArtist").Value;
         _repositoryMock.GetByIdAsync(Arg.Is<ArtistId>(a => a.Value == guid), Arg.Any<CancellationToken>()).Returns(artist);
        var command = new DeleteArtistCommand(guid);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(artist.IsActive);
        await _repositoryMock.Received(1).GetByIdAsync(Arg.Is<ArtistId>(a => a.Value == guid), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    [Fact]
    public async Task Handle_ArtistIdNotFound_ReturnsFailure()
    {
        var guid = Guid.NewGuid();
        _repositoryMock.GetByIdAsync(Arg.Any<ArtistId>(), Arg.Any<CancellationToken>()).ReturnsNull();
        var command = new DeleteArtistCommand(guid);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Artist.KeyNotFound, result.Error);

        await _repositoryMock.Received(1).GetByIdAsync(Arg.Is<ArtistId>(a => a.Value == guid), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

    }

    [Fact]
    public async Task Handle_DeactivateAlreadyInactive_ReturnsFailure()
    {
        var guid = Guid.NewGuid();
        var artist = Artist.Create(new ArtistId(guid), "TestArtist").Value;
        artist.DeActivate();
        _repositoryMock.GetByIdAsync(Arg.Is<ArtistId>(a => a.Value == guid), Arg.Any<CancellationToken>()).Returns(artist);
        var command = new DeleteArtistCommand(guid);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(Errors.Artist.AlreadyInactive, result.Error);
        await _repositoryMock.Received(1).GetByIdAsync(Arg.Is<ArtistId>(a => a.Value == guid), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

    }
}
