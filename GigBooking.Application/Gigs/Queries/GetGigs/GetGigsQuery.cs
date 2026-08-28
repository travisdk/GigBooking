using GigBooking.Application.Common.Interfaces;
using GigBooking.Application.Gigs.DTOs;
using GigBooking.Application.Venues.DTOs;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GigBooking.Application.Venues.Queries.GetVenues;

public sealed record GetGigsQuery() : IRequest<Result<IEnumerable<GigDetailsDTO>>>;
public class GetGigsQueryHandler : IRequestHandler<GetGigsQuery, Result<IEnumerable<GigDetailsDTO>>>
{
    private readonly IGigReadRepository _repository;

    public GetGigsQueryHandler(IGigReadRepository repository) { 
    
        _repository = repository;

    }
    public async Task<Result<IEnumerable<GigDetailsDTO>>> Handle(GetGigsQuery request, CancellationToken ctx)
    {
        var gigs = await _repository.GetAllGigsWithDetailsAsync(ctx);
        return Result<IEnumerable<GigDetailsDTO>>.Success(gigs);
    }
}
