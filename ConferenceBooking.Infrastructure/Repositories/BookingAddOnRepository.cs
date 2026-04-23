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
                .Where(ba => ba.BookingId == bookingId) //gets booking add on relation with booking id
                .SumAsync(ba => ba.BookedPricePerPerson * ba.Quantity); //takes sum of all add ons in booking and returns cost

        public async Task<List<BookingAddOn>> GetAllBookingAddOnsRowsWithAddOns()
            => await dbSet.Include(ba => ba.AddOn).ToListAsync();
    }
}
