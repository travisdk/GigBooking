using GigBooking.Application.Artists.DTOs;
using MediatR;
using GigBooking.Domain.Primitives;

using GigBooking.Domain.Abstractions;

namespace GigBooking.Application.Artists.Queries.GetArtists;

public sealed record GetArtistsQuery() : IRequest<Result<IEnumerable<ArtistDTO>>>;


public class GetArtistsQueryHandler : IRequestHandler<GetArtistsQuery, Result<IEnumerable<ArtistDTO>>>

{
    private readonly IArtistRepository _repository;

    public GetArtistsQueryHandler(IArtistRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<ArtistDTO>>> Handle(GetArtistsQuery request, CancellationToken ctx)
    {

        var artists = await _repository.GetAllArtistsAsync(ctx);
        var result = artists.ToList().Select(artist => new ArtistDTO(artist.Id.Value, artist.Name));

        return Result<IEnumerable<ArtistDTO>>.Success(result);

    }
}
