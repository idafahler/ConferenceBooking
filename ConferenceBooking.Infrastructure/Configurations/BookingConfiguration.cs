using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ConferenceBooking.Infrastructure.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Property(b => b.TotalPrice)
                .HasColumnType(SqlDbType.Money.ToString());

            builder.Property(b => b.StartTime)
                .HasColumnType("datetime2(0)");

            builder.Property(b => b.EndTime)
                .HasColumnType("datetime2(0)");

            builder.Property(b => b.CreatedAt)
                .HasColumnType("datetime2(0)");

            builder.Property(b => b.UserId)
                .IsRequired(false);

            builder.HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(b => b.ConferenceRoom)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.ConferenceRoomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.BookingAddOns)
                .WithOne(ba => ba.Booking)
                .HasForeignKey(ba => ba.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
