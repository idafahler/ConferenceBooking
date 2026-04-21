using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Application.RepositoryInterfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<List<Booking>> GetBookingsByUserAsync(int userId);

        Task<List<Booking>> GetAllBookingsForUserWithDetailsAsync(int userId);

        Task<Booking?> GetBookingByIdWithDetailsAsync(int id);
    }
}
