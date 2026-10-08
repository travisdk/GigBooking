using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GigBooking.Infrastructure.Persistence.Repositories;

public class VenueRepository : IVenueRepository
{

    private readonly AppDbContext _context;
    public VenueRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Venue>> GetAllVenuesAsync(CancellationToken ctx)
    {
        var venues = await _context.Set<Venue>().Where(v => v.IsActive).AsNoTracking().ToListAsync(ctx);
        return venues;
    }

    public async Task<Venue?> GetByIdAsync(VenueId id, CancellationToken ctx = default)
    {
        var venue = await _context.Set<Venue>().FirstOrDefaultAsync(v => v.Id == id, ctx);
        if (venue == null)
        {
            return null;
        }
        return venue;
    } 
    
    public async Task<Venue?> GetByIdReadOnlyAsync(VenueId id, CancellationToken ctx = default)
    {
        var venue = await _context.Set<Venue>().AsNoTracking().FirstOrDefaultAsync(v => v.Id == id, ctx);

        if (venue == null)
        {
            return null;
        }

        return venue;
    }
    public Venue Add(Venue venue)
    {
        _context.Set<Venue>().Add(venue);
        return venue;
    }
}
