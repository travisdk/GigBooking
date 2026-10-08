using GigBooking.Application.Venues.DTOs;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GigBooking.Application.Venues.Queries.GetVenues;

public sealed record GetVenuesQuery() : IRequest<Result<IEnumerable<VenueDTO>>>;
public class GetVenuesQueryHandler : IRequestHandler<GetVenuesQuery, Result<IEnumerable<VenueDTO>>>
{
    private readonly IVenueRepository _repository;

    public GetVenuesQueryHandler(IVenueRepository repository) { 
    
        _repository = repository;

    }
    public async Task<Result<IEnumerable<VenueDTO>>> Handle(GetVenuesQuery request, CancellationToken ctx)
    {
        var venues = await _repository.GetAllVenuesAsync(ctx);

        var dtos = venues.Select(v => new VenueDTO(v.Id.Value, v.Name, 
              LocationDTO.FromDomain(v.Location), v.IsActive)).ToList();

        return Result<IEnumerable<VenueDTO>>.Success(dtos);
    }
}
