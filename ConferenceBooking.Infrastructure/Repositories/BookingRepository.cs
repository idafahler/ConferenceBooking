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

        public async Task<List<Booking>> GetAllBookingsForUserWithDetailsAsync(int userId)
            => await dbSet
                .Include(b => b.ConferenceRoom)
                .Include(b => b.BookingAddOns)
                    .ThenInclude(ba => ba.AddOn)
                .Include(b => b.User)
                .Where(b => b.UserId == userId)
                .ToListAsync();

        public async Task<Booking?> GetBookingByIdWithDetailsAsync(int id)
            => await dbSet
            .Include(b => b.ConferenceRoom)
            .Include(b => b.BookingAddOns)
                .ThenInclude(ba => ba.AddOn)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
}
