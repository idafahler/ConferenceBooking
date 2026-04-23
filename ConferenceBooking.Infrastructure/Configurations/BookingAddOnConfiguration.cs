using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ConferenceBooking.Infrastructure.Configurations
{
    public class BookingAddOnConfiguration : IEntityTypeConfiguration<BookingAddOn>
    {
        public void Configure(EntityTypeBuilder<BookingAddOn> builder)
        {
            builder.Property(ba => ba.BookedPricePerPerson)
                .HasColumnType(SqlDbType.Money.ToString());

            builder.HasOne(ba => ba.AddOn)
                .WithMany(a => a.BookingAddOns) //many to many relation between booking and add on
                .HasForeignKey(ba => ba.AddOnId)
                .OnDelete(DeleteBehavior.Restrict); //cannot delete a add on that is tied to a booking
        }
    }
}
