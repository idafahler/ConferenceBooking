using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ConferenceBooking.Domain.Interfaces;

namespace ConferenceBooking.Infrastructure.Repositories
{
    public class BookingRepository(ConferenceBookingContext context) : Repository<Booking>(context), IBookingRepository
    {
        public async Task<List<Booking>> GetBookingsByUserAsync(int userId)
            => await dbSet.Where(b => b.UserId == userId).ToListAsync();
    }
}
