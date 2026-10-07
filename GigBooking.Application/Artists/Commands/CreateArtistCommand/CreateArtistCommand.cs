using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using GigBooking.Domain.Entities;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.Abstractions;
using GigBooking.Application.Artists.DTOs;
using GigBooking.Application.Common.Interfaces;
using GigBooking.Domain.ValueObjects;
using GigBooking.Domain;
using FluentValidation;
namespace GigBooking.Application.Artists.Commands.CreateArtistCommand;

public sealed record CreateArtistCommand(string Name) : IRequest<Result<ArtistDTO>>;


public  class CreateArtistCommandHandler : IRequestHandler<CreateArtistCommand, Result<ArtistDTO>>
{
    private readonly IArtistRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGuidGenerator _guidGenerator;

    public CreateArtistCommandHandler(IArtistRepository repository, IUnitOfWork unitOfWork, IGuidGenerator guidGenerator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _guidGenerator = guidGenerator;
    }

    public async Task<Result<ArtistDTO>> Handle(CreateArtistCommand request, CancellationToken cancellationToken)
    {
        var newGuid = _guidGenerator.GenerateGuid();
        var artistResult = Artist.Create(new ArtistId(newGuid), request.Name);
        
        if (artistResult.IsFailure)
        {
            return Result<ArtistDTO>.Failure(artistResult.Error!);
        }
        
        Artist artist = artistResult.Value; 
        _repository.Add(artist);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ArtistDTO>.Success(new ArtistDTO(artist.Id.Value, artist.Name, artist.IsActive)  );
    }
    
}

public class CreateArtistCommandValidator : AbstractValidator<CreateArtistCommand>
{
        public CreateArtistCommandValidator() {

            RuleFor(x => x.Name).NotEmpty().WithErrorCode(Errors.Artist.NoName.Code).WithMessage((
                Errors.Artist.NoName.Description));
    
        }
}
