using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Application.RepositoryInterfaces
{
    public interface IBookingAddOnRepository : IRepository<BookingAddOn>
    {
        Task<decimal> CalculateTotalAddonPriceAsync(int bookingId);
        Task<List<BookingAddOn>> GetAllBookingAddOnsRowsWithAddOns();
    }
}
