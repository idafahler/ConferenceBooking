using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ConferenceBooking.Infrastructure.Configurations
{
    public class AddOnConfiguration : IEntityTypeConfiguration<AddOn>
    {
        public void Configure(EntityTypeBuilder<AddOn> builder)
        {
            builder.Property(ao => ao.Name).HasMaxLength(64);
            builder.HasIndex(ao => ao.Name).IsUnique();

            builder.Property(ao => ao.PricePerPerson)
                .HasColumnType(SqlDbType.Money.ToString());

            builder.HasData(
                new AddOn { Id = 1, Name = "Coffee", PricePerPerson = 20},
                new AddOn { Id = 2, Name = "Pastries", PricePerPerson = 35},
                new AddOn { Id = 3, Name = "Fruit basket", PricePerPerson = 25},
                new AddOn { Id = 4, Name = "Water", PricePerPerson = 10},
                new AddOn { Id = 5, Name = "Lunch catering", PricePerPerson = 80}
                );
        }
    }
}
