using GigBooking.Application.Common.Interfaces;
using GigBooking.Application.Gigs.DTOs;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Errors;
using GigBooking.Domain.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Application.Gigs.Queries.GetGig;

public sealed record GetGigQuery(Guid Id) : IRequest<Result<GigDTO>>;


public class GetGigQueryHandler : IRequestHandler<GetGigQuery, Result<GigDTO>>

{
    private readonly IGigRepository _repository;
    public GetGigQueryHandler(IGigRepository repository)
    {
        _repository = repository;
    }
    public async Task<Result<GigDTO>> Handle(GetGigQuery request, CancellationToken ctx)
    {
        var gig = await _repository.GetByIdReadOnlyAsync(new Domain.ValueObjects.GigId(request.Id), ctx);
        if (gig == null)
        {
            return Result<GigDTO>.Failure(DomainErrors.Gig.KeyNotFound);
        }
        return Result<GigDTO>.Success(new GigDTO(gig.Id.Value, gig.ArtistId.Value, gig.VenueId.Value, gig.ScheduledAt));
    }
}