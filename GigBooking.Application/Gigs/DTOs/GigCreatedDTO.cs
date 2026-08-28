using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Application.Gigs.DTOs;

public sealed record GigDTO(Guid Id, Guid ArtistId, Guid VenueId, DateTime ScheduledAt);
