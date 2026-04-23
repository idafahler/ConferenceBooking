using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ConferenceBooking.Application.RepositoryInterfaces;

namespace ConferenceBooking.Infrastructure.Repositories
{
    public class BookingRepository(ConferenceBookingContext context) : Repository<Booking>(context), IBookingRepository
    {
        public async Task<List<Booking>> GetBookingsByUserAsync(int userId)
            => await dbSet.Where(b => b.UserId == userId).ToListAsync();

        public async Task<List<Booking>> GetAllBookingsForUserWithDetailsAsync(int userId) //gets all bookings with details based on user id
            => await dbSet
                .Include(b => b.ConferenceRoom) //including conference room
                .Include(b => b.BookingAddOns) //including booking add on 
                    .ThenInclude(ba => ba.AddOn) //including add ons
                .Include(b => b.User) //including user
                .Where(b => b.UserId == userId)
                .ToListAsync();

        public async Task<Booking?> GetBookingByIdWithDetailsAsync(int id) //gets booking with all details
            => await dbSet
            .Include(b => b.ConferenceRoom)
            .Include(b => b.BookingAddOns)
                .ThenInclude(ba => ba.AddOn)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
}
