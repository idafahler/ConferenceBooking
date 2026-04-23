using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Infrastructure
{
    public class ConferenceBookingContext : DbContext
    {
        public DbSet<AddOn> AddOns { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingAddOn> BookingAddOns { get; set; } 
        public DbSet<ConferenceRoom> ConferenceRooms { get; set; }
        public DbSet<RoomFeature> RoomFeatures { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder() //connection string in appsettings.json
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString =
                config.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConferenceBookingContext).Assembly); //applying all configurations

            modelBuilder.UseCollation("Finnish_Swedish_CI_AS");
        }
    }
}
