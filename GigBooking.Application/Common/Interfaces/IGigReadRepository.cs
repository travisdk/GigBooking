using GigBooking.Application.Gigs.DTOs;
using GigBooking.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Application.Common.Interfaces;

public interface IGigReadRepository  // pure Query repo with join needs
{
    Task<IEnumerable<GigDetailsDTO>> GetAllGigsWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<GigDetailsDTO?> GetGigWithDetailsByIdAsync(GigId id, CancellationToken cancellationToken = default);
}
