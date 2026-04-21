using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConferenceBooking.Application.ServiceInterfaces
{
    public interface IBookingService
    {
        Task<List<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<IEnumerable<Booking>> FindBookingsAsync(Expression<Func<Booking, bool>> condition);
        Task<ServiceResult> CreateBookingAsync(User user, int roomId, DateTime start, DateTime end, List<int> addOns);
        Task<ServiceResult> UpdateBookingAsync(Booking booking);
        Task<ServiceResult> DeleteBookingAsync(int id);
        Task<List<Booking>> GetAllBookingsForUserWithDetailsAsync(int userId);
        Task<Booking?> GetBookingByIdWithDetailsAsync(int id);

    }
}
