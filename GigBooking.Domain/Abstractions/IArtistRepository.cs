using System;
using System.Collections.Generic;
using System.Text;
using GigBooking.Domain.Entities;
namespace GigBooking.Domain.Abstractions;

public interface IArtistRepository
{
    public Artist Add(Artist artist);
    public Task<Artist?> GetByIdAsync(Guid id, CancellationToken ctx);
    public Task<Artist?> GetByIdReadOnlyAsync(Guid id, CancellationToken ctx);
}
