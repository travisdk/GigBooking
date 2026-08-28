using GigBooking.Domain.Entities;
using GigBooking.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Infrastructure.Persistence.Configurations;

public sealed class GigConfiguration : IEntityTypeConfiguration<Gig>
{
    public void Configure(EntityTypeBuilder<Gig> builder)
    {
        builder.ToTable("Gigs");
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id).HasConversion(id => id.Value, value => new GigId(value));
        builder.Property(g => g.ArtistId).HasConversion(id => id.Value, value => new ArtistId(value));
        builder.Property(g => g.VenueId).HasConversion(id => id.Value, value => new VenueId(value));
        builder.Property(g => g.ScheduledAt).IsRequired();

        builder.HasOne<Artist>().WithMany().HasForeignKey(g => g.ArtistId);
        builder.HasOne<Venue>().WithMany().HasForeignKey(g => g.VenueId);
    }
}
