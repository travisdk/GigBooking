using System;
using System.Collections.Generic;
using System.Text;
using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;
namespace GigBooking.Domain.Abstractions;

public interface IVenueRepository
{
    public Venue Add(Venue venue);
    public Task<Venue?> GetByIdAsync(VenueId id, CancellationToken ctx);
    public Task<Venue?> GetByIdReadOnlyAsync(VenueId id, CancellationToken ctx);
    public Task<IEnumerable<Venue>> GetAllVenuesAsync(CancellationToken ctx);
}
