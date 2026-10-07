using FluentValidation;
using GigBooking.Application.Artists.Commands.CreateArtistCommand;
using GigBooking.Application.Common.Interfaces;
using GigBooking.Application.Venues.DTOs;
using GigBooking.Domain;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Entities;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
namespace GigBooking.Application.Venues.Commands.CreateVenueCommand;

public sealed record CreateVenueCommand(string Name, string Street, 
       string PostalCode, string City, string CountryCode) : IRequest<Result<VenueDTO>>;


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
        var locationResult = Location.Create(request.Street, request.PostalCode, request.City, request.CountryCode);

        if (locationResult.IsFailure)
        {
            return Result<VenueDTO>.Failure(locationResult.Error!);
        }



        var newGuid = _guidGenerator.GenerateGuid();

        var venueResult = Venue.Create(new VenueId(newGuid), request.Name, locationResult.Value);
        
        if (venueResult.IsFailure)
        {
            return Result<VenueDTO>.Failure(venueResult.Error!);
        }
        
        Venue venue = venueResult.Value; 
        _repository.Add(venue);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var locationDTO = new LocationDTO(
             venue.Location.Street, venue.Location.PostalCode, venue.Location.City, venue.Location.CountryCode
        );

        return Result<VenueDTO>.Success(new VenueDTO(venue.Id.Value, venue.Name, locationDTO)  );
    }

}
public class CreateVenueCommandValidator : AbstractValidator<CreateVenueCommand>
{
    public CreateVenueCommandValidator()
    {

        RuleFor(x => x.Name).NotEmpty().WithErrorCode(Errors.Venue.NoName.Code).WithMessage((
            Errors.Venue.NoName.Description));
        RuleFor(x => x.Street).NotEmpty().WithErrorCode(Errors.Location.InvalidFields.Code)
            .WithMessage(Errors.Location.InvalidFields.Description);
        RuleFor(x => x.PostalCode).NotEmpty().WithErrorCode(Errors.Location.InvalidFields.Code)
            .WithMessage(Errors.Location.InvalidFields.Description);
        RuleFor(x => x.City).NotEmpty().WithErrorCode(Errors.Location.InvalidFields.Code)
             .WithMessage(Errors.Location.InvalidFields.Description);
        RuleFor(x => x.CountryCode).NotEmpty().Length(2).WithErrorCode(Errors.Location.InvalidCountryCode.Code)
            .WithMessage(Errors.Location.InvalidCountryCode.Description);
        
    }
}
