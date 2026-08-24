using GigBooking.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Infrastructure.Data;

public class AppDbContext : DbContext, IUnitOfWork
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    //public async Task<int> SaveChangesAsync(CancellationToken ctx)
    //{
    //    return await base.SaveChangesAsync(ctx);
    //}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);

    }
}
