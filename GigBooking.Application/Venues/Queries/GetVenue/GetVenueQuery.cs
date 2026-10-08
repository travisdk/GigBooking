using FluentValidation;
using GigBooking.Application.Artists.Queries.GetArtist;
using GigBooking.Application.Venues.DTOs;
using GigBooking.Domain;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.ValueObjects;
using MediatR;

namespace GigBooking.Application.Venues.Queries.GetVenue;

public sealed record GetVenueQuery(Guid Id) : IRequest<Result<VenueDTO>>;


public class GetVenueQueryHandler : IRequestHandler<GetVenueQuery, Result<VenueDTO>>
{
    private readonly IVenueRepository _repository;

    public GetVenueQueryHandler(IVenueRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<VenueDTO>> Handle(GetVenueQuery request, CancellationToken ctx)
    {

        var venue = await _repository.GetByIdReadOnlyAsync(new VenueId(request.Id), ctx);
        if (venue == null)
        {
            return Result<VenueDTO>.Failure(Errors.Venue.KeyNotFound);
        }

        LocationDTO locationDTO = LocationDTO.FromDomain(venue.Location);

        return Result<VenueDTO>.Success(new VenueDTO(venue.Id.Value, venue.Name, locationDTO, venue.IsActive));

    }
}
public class GetVenueQueryValidator : AbstractValidator<GetVenueQuery>
{

    public GetVenueQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode(Errors.Venue.EmptyVenueId.Code);
    }
}

