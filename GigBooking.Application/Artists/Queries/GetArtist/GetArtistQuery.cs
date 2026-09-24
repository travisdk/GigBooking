using MediatR;
using GigBooking.Domain.Primitives;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.ValueObjects;
using GigBooking.Application.Artists.DTOs;
using GigBooking.Domain;
using FluentValidation;
using System.Data;

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
            return Result<ArtistDTO>.Failure(Errors.Artist.KeyNotFound);
        }

        return Result<ArtistDTO>.Success(new ArtistDTO(artist.Id.Value, artist.Name));

    }

    public class GetArtistQueryValidator : AbstractValidator<GetArtistQuery> {

        public GetArtistQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithErrorCode(Errors.Artist.EmptyArtistId.Code);
        }
    }
}
