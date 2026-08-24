using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Domain.Primitives;

public sealed record Error(string Code, string? Description = null);