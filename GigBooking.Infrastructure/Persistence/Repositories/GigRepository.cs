using GigBooking.Application.Common.Interfaces;
using GigBooking.Application.Gigs.DTOs;
using GigBooking.Domain.Abstractions;
using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace GigBooking.Infrastructure.Persistence.Repositories;

public class GigRepository : IGigRepository, IGigReadRepository
{
    private readonly AppDbContext _context;

    public GigRepository(AppDbContext context) {
        
        _context = context;
    }
    public Gig Add(Gig gig)
    {
        _context.Set<Gig>().Add(gig);
        return gig;
    }

    public async Task<Gig?> GetByIdAsync(GigId id, CancellationToken ctx)
    {
        var gig =  await _context.Set<Gig>().FirstOrDefaultAsync(g => g.Id == id, ctx);
        if (gig == null)
        {
            return null;
        }
        return gig;
    }
    public async Task<Gig?> GetByIdReadOnlyAsync(GigId id, CancellationToken ctx)
    {
        var gig = await _context.Set<Gig>().AsNoTracking().FirstOrDefaultAsync(g => g.Id == id, ctx);
        if (gig == null)
        {
            return null;
        }
        return gig;
    }


    public async Task<IEnumerable<GigDetailsDTO>> GetAllGigsWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        var gigs = await _context.Set<Gig>()
            .AsNoTracking()
            .Select(gig => new GigDetailsDTO(
                gig.Id.Value, gig.ArtistId.Value, gig.VenueId.Value, gig.ScheduledAt,
                  _context.Set<Artist>().First(a => a.Id == gig.ArtistId).Name,
                  _context.Set<Venue>().First(v => v.Id == gig.VenueId).Name

                )
            ).ToListAsync(cancellationToken);

        return gigs;
    }

    public Task<GigDetailsDTO?> GetGigWithDetailsByIdAsync(GigId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Gig>> GetGigsByDateAsync(DateTimeOffset date, CancellationToken ctx)
    {
        var startOfDay = date.Date;
        var endOfDay = date.AddDays(1);
        return await _context.Set<Gig>().AsNoTracking().Where(g => g.ScheduledAt >= startOfDay
          && g.ScheduledAt < endOfDay )
            .ToListAsync(ctx);
    }
}
