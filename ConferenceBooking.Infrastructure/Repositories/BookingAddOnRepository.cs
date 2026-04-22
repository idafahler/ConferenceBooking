using ConferenceBooking.Application.RepositoryInterfaces;
using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Infrastructure.Repositories
{
    public class BookingAddOnRepository(ConferenceBookingContext context) : Repository<BookingAddOn>(context), IBookingAddOnRepository
    {
        public async Task<decimal> CalculateTotalAddonPriceAsync(int bookingId) =>
            await dbSet
                .Where(ba => ba.BookingId == bookingId)
                .SumAsync(ba => ba.BookedPricePerPerson * ba.Quantity);

        public async Task<List<BookingAddOn>> GetAllWithAddonAsync() 
            => await dbSet.Include(ba => ba.AddOn).ToListAsync();
    }
}
