using GigBooking.Application.Common.Interfaces;
using GigBooking.Application.Gigs.DTOs;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;
using GigBooking.Domain.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Application.Gigs.Commands;

public sealed record CreateGigCommand(Guid ArtistId, Guid VenueId, DateTime ScheduledAt) : IRequest<Result<GigDTO>>;

public class CreateGigCommandHandler : IRequestHandler<CreateGigCommand, Result<GigDTO>>
{
    private readonly IGigRepository _repository;
    private readonly IArtistRepository _artistRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGigCommandHandler(IGigRepository repository, IArtistRepository artistRepository, 
        IVenueRepository venueRepository, IUnitOfWork unitOfWork, IGuidGenerator guidGenerator)
    {
        _repository = repository;
        _artistRepository = artistRepository;
        _venueRepository = venueRepository;
        _guidGenerator = guidGenerator;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<GigDTO>> Handle(CreateGigCommand request, CancellationToken cancellationToken)
    {
       
        // DOMAIN LOGIC NEEDS TO BE IMPLEMENTED TO CHECK VALIDITIES LATER
        if (await _artistRepository.GetByIdReadOnlyAsync(new ArtistId(request.ArtistId),
              cancellationToken) == null)
        {
            return Result<GigDTO>.Failure(DomainErrors.Artist.KeyNotFound);
        }

        if (await _venueRepository.GetByIdReadOnlyAsync(new VenueId(request.VenueId), cancellationToken) == null)
        {
            return Result<GigDTO>.Failure(DomainErrors.Venue.KeyNotFound);
        }
        var gigId = new GigId(_guidGenerator.GenerateGuid());
        var gigResult = Gig.Create(gigId, new ArtistId(request.ArtistId), new VenueId(request.VenueId)
              , request.ScheduledAt);
        

        if (gigResult.IsFailure)
        {
            return Result<GigDTO>.Failure(gigResult.Error!);
        }
        var gig = gigResult.Value;
        _repository.Add(gig);
      
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        GigDTO gigDTO = new GigDTO(gig.Id.Value, gig.ArtistId.Value,
              gig.VenueId.Value, gig.ScheduledAt); 
        return Result<GigDTO>.Success(gigDTO);          
        
    }
}

