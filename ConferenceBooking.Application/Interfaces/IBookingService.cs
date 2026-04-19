using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConferenceBooking.Application.Interfaces
{
    public interface IBookingService
    {
        Task<List<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<IEnumerable<Booking>> FindBookingsAsync(Expression<Func<Booking, bool>> condition);
        Task<ServiceResult> CreateBookingAsync(Booking booking);
        Task<ServiceResult> UpdateBookingAsync(Booking booking);
        Task<ServiceResult> DeleteBookingAsync(int id);
    }
}
