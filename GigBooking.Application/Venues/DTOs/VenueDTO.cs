using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Application.Venues.DTOs;

public sealed record VenueDTO(Guid Id, string Name, LocationDTO Location, bool IsActive);   