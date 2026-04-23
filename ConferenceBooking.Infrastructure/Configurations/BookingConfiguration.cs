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
                .HasColumnType("datetime2(0)"); // datetime2(0), no decimals for seconds is stored

            builder.Property(b => b.EndTime)
                .HasColumnType("datetime2(0)");

            builder.Property(b => b.CreatedAt)
                .HasColumnType("datetime2(0)");

            builder.Property(b => b.UserId)
                .IsRequired(false);

            builder.HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict); //cannot delete user with booking relation

            builder.HasOne(b => b.ConferenceRoom) //conference room with booking cannot be deleted
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.ConferenceRoomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.BookingAddOns) //if a booking is deleted all its data in bookingaddon table is also removed.
                .WithOne(ba => ba.Booking)
                .HasForeignKey(ba => ba.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
