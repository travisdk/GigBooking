using GigBooking.Application.Artists.DTOs;
using MediatR;
using GigBooking.Domain.Entities;
using GigBooking.Domain.Primitives;

using System;
using System.Collections.Generic;
using System.Text;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Errors;
using GigBooking.Application.Venues.DTOs;
using GigBooking.Domain.ValueObjects;

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
            return Result<VenueDTO>.Failure(DomainErrors.Venue.KeyNotFound);
        }

        return Result<VenueDTO>.Success(new VenueDTO(venue.Id.Value, venue.Name));

    }
}
