using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Domain.ValueObjects;

public readonly record struct VenueId(Guid Value);
