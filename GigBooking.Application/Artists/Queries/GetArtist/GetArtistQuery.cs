using GigBooking.Application.Artists.DTOs;
using MediatR;
using GigBooking.Domain.Entities;
using GigBooking.Domain.Primitives;

using System;
using System.Collections.Generic;
using System.Text;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Errors;
using GigBooking.Domain.ValueObjects;

namespace GigBooking.Application.Artists.Queries.GetArtist;

public sealed record GetArtistQuery(Guid Id) : IRequest<Result<ArtistDTO>>;


public class GetArtistQueryHandler : IRequestHandler<GetArtistQuery, Result<ArtistDTO>>

{
    private readonly IArtistRepository _repository;

    public GetArtistQueryHandler(IArtistRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ArtistDTO>> Handle(GetArtistQuery request, CancellationToken ctx)
    {

        var artist = await _repository.GetByIdReadOnlyAsync(new ArtistId(request.Id), ctx);
        if (artist == null)
        {
            return Result<ArtistDTO>.Failure(DomainErrors.Artist.KeyNotFound);
        }

        return Result<ArtistDTO>.Success(new ArtistDTO(artist.Id.Value, artist.Name));

    }
}
