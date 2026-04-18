using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Infrastructure.Configurations
{
    public class RoomFeatureConfiguration : IEntityTypeConfiguration<RoomFeature>
    {
        public void Configure(EntityTypeBuilder<RoomFeature> builder)
        {
            builder.Property(f => f.Name)
                .HasMaxLength(64);

            builder.HasIndex(f => f.Name).IsUnique();

            builder.HasData(
                new { Id = 1, Name = "Projector"},
                new { Id = 2, Name = "Whiteboard"},
                new { Id = 3, Name = "Video conferencing"},
                new { Id = 4, Name = "Ocean view"}
                );
        }
    }
}
