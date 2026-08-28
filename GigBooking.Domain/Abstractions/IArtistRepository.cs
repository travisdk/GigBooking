using System;
using System.Collections.Generic;
using System.Text;
using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;
namespace GigBooking.Domain.Abstractions;

public interface IArtistRepository
{
    public Artist Add(Artist artist);
    public Task<Artist?> GetByIdAsync(ArtistId id, CancellationToken ctx);
    public Task<Artist?> GetByIdReadOnlyAsync(ArtistId id, CancellationToken ctx);

    public Task<IEnumerable<Artist>> GetAllArtistsAsync(CancellationToken ctx);
}
