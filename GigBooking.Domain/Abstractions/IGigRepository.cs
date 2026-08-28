using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Domain.Abstractions;

public interface IGigRepository // pure Command repo with no join needs
{
    public Gig Add(Gig gig);
    public Task<Gig?> GetByIdAsync(GigId id, CancellationToken ctx);
    public Task<Gig?> GetByIdReadOnlyAsync(GigId id, CancellationToken ctx);
}
