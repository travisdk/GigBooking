using GigBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Infrastructure.Configurations;

public sealed class ArtistConfiguration : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> builder)
    {

        builder.ToTable("Artists");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).IsRequired();


    }
}
