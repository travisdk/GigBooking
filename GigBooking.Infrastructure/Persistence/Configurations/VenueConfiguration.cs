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

    }
}
