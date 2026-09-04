using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Application.Gigs.DTOs;

public sealed record GigDetailsDTO(
    Guid Id,
    Guid ArtistId,
    Guid VenueId,
    DateTimeOffset ScheduledAt,
    string ArtistName,
    string VenueName
);