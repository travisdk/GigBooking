using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GigBooking.Infrastructure.Persistence.Repositories;

public class ArtistRepository : IArtistRepository
{

    private readonly AppDbContext _context;
    public ArtistRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Artist>> GetAllArtistsAsync(CancellationToken ctx)
    {
       var artists = await _context.Set<Artist>().AsNoTracking().ToListAsync(ctx);
       return artists;
    }
    public async Task<Artist?> GetByIdAsync(ArtistId id, CancellationToken ctx = default)
    {
        var artist = await _context.Set<Artist>().FirstOrDefaultAsync(a => a.Id == id, ctx);
        if (artist == null)
        {
            return null;
        }
        return artist;
    } 
    
    public async Task<Artist?> GetByIdReadOnlyAsync(ArtistId id, CancellationToken ctx = default)
    {
        var artist = await _context.Set<Artist>().AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ctx);

        if (artist == null)
        {
            return null;
        }

        return artist;
    }
    public Artist Add(Artist artist)
    {
        _context.Set<Artist>().Add(artist);
        return artist;
    }


}
