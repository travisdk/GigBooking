using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Infrastructure.Persistence.Configurations;

public sealed class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {

        builder.ToTable("Venues");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).HasConversion(id => id.Value, value => new VenueId(value));
        builder.Property(v => v.Name).IsRequired();
        builder.ComplexProperty(v => v.Location, loc =>
        {
            loc.Property(l => l.Street).IsRequired();
            loc.Property(l => l.PostalCode).IsRequired();
            loc.Property(l => l.City).IsRequired();
            loc.Property(l => l.CountryCode).HasMaxLength(2).IsFixedLength().IsRequired();

        });
        builder.Property(v => v.IsActive).IsRequired();

    }
}
