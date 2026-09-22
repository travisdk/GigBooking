using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Domain.Primitives;

public interface IValidationResult<out TResult>
{
    static abstract TResult WithError(Error error);
}