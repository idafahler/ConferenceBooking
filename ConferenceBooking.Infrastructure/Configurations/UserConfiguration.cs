using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasDiscriminator<string>("Role")
                .HasValue<Admin>("Admin")
                .HasValue<Employee>("Employee")
                .HasValue<ExternalUser>("ExternalUser");

            builder.Property(u => u.Username)
                .HasMaxLength(64);
            builder.Property(u => u.PasswordHash)
                .HasMaxLength(128);
            builder.Property(u => u.FirstName)
                .HasMaxLength(64);
            builder.Property(u => u.LastName)
                .HasMaxLength(64);
            builder.Property(u => u.Email)
                .HasMaxLength(64);

            builder.HasIndex(u => u.Username).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}
