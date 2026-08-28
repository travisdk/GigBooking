using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using GigBooking.Domain.Entities;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.Abstractions;
using GigBooking.Application.Venues.DTOs;
using GigBooking.Application.Common.Interfaces;
using GigBooking.Domain.ValueObjects;
namespace GigBooking.Application.Venues.Commands.CreateVenueCommand;

public sealed record CreateVenueCommand(string Name) : IRequest<Result<VenueDTO>>;


public  class CreateVenueCommandHandler : IRequestHandler<CreateVenueCommand, Result<VenueDTO>>
{
    private readonly IVenueRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGuidGenerator _guidGenerator;

    public CreateVenueCommandHandler(IVenueRepository repository, IUnitOfWork unitOfWork, IGuidGenerator guidGenerator)
    {

        _repository = repository;
        _unitOfWork = unitOfWork;
        _guidGenerator = guidGenerator;
    }

    public async Task<Result<VenueDTO>> Handle(CreateVenueCommand request, CancellationToken cancellationToken)
    {
        var newGuid = _guidGenerator.GenerateGuid();

        var venueResult = Venue.Create(new VenueId(newGuid), request.Name);
        
        if (venueResult.IsFailure)
        {
            return Result<VenueDTO>.Failure(venueResult.Error!);
        }
        
        Venue venue = venueResult.Value; 
        _repository.Add(venue);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<VenueDTO>.Success(new VenueDTO(venue.Id.Value, venue.Name)  );
    }

}
