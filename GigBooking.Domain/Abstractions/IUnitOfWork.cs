using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Domain.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ctx);
}
