using ConferenceBooking.Application.Models;
using ConferenceBooking.Application.ServiceInterfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Infrastructure
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceScopeFactory scopeFactory)
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ConferenceBookingContext>();

            var existingUsers = await context.Users.CountAsync();
            if (existingUsers > 0)
                return;

            var admin = new Admin("admin", BCrypt.Net.BCrypt.HashPassword("Admin123!"), "Admin", "Adminsson", "admin@conference.com");
            var maria = new Employee("maria.hr", BCrypt.Net.BCrypt.HashPassword("Maria123!"), "Maria", "Johansson", "maria@conference.com", Department.HR);
            var lisa = new ExternalUser("lisaikea", BCrypt.Net.BCrypt.HashPassword("Lisa123!"), "Lisa", "Nilsson", "lisa@ikea.com", "IKEA");

            context.AddRange(admin, maria, lisa);
            await context.SaveChangesAsync();

            var friday = GetNextWeekday(DayOfWeek.Friday);
            var nextMonday = GetNextWeekday(DayOfWeek.Monday, nextWeek: true);
            var nextTuesday = GetNextWeekday(DayOfWeek.Tuesday, nextWeek: true);

            var room1 = await context.ConferenceRooms.FindAsync(1);
            var room2 = await context.ConferenceRooms.FindAsync(2);

            context.AddRange(
                new Booking
                {
                    User = maria,
                    ConferenceRoomId = 1,
                    StartTime = friday.AddHours(13),
                    EndTime = friday.AddHours(16),
                    TotalPrice = 0,
                    CreatedAt = DateTime.Now
                },
                new Booking
                {
                    User = maria,
                    ConferenceRoomId = 2,
                    StartTime = nextMonday.AddHours(8),
                    EndTime = nextMonday.AddHours(17),
                    TotalPrice = 0,
                    CreatedAt = DateTime.Now
                },
                new Booking
                {
                    User = lisa,
                    ConferenceRoomId = 2,
                    StartTime = nextTuesday.AddHours(10),
                    EndTime = nextTuesday.AddHours(14),
                    TotalPrice = 4 * room2!.PricePerHour,
                    CreatedAt = DateTime.Now
                });

            await context.SaveChangesAsync();
        }

        private static DateTime GetNextWeekday(DayOfWeek day, bool nextWeek = false) //Dynamic dates
        {
            var today = DateTime.Today;
            var diff = (int)day - (int)today.DayOfWeek;
            if (diff <= 0 || nextWeek) diff += 7;
            return today.AddDays(diff);
        }
    }
}
