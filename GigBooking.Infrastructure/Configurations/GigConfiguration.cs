using GigBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Infrastructure.Configurations;

public sealed class GigConfiguration : IEntityTypeConfiguration<Gig>
{
    public void Configure(EntityTypeBuilder<Gig> builder)
    {
        builder.ToTable("Gigs");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.ScheduledAt).IsRequired();

        builder.HasOne<Artist>().WithMany().HasForeignKey(g => g.ArtistId);
        builder.HasOne<Venue>().WithMany().HasForeignKey(g => g.VenueId);
    }
}
