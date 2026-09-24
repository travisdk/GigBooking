using MediatR;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.Abstractions;
using GigBooking.Application.Venues.DTOs;
using GigBooking.Domain.ValueObjects;
using GigBooking.Domain;

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

        return Result<VenueDTO>.Success(new VenueDTO(venue.Id.Value, venue.Name));

    }
}
