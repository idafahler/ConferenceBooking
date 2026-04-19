using ConferenceBooking.Application.Interfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConferenceBooking.Application.Services
{
    public class BookingService(IBookingRepository repo) : IBookingService
    {
        public async Task<List<Booking>> GetAllBookingsAsync()
            => await repo.GetAllAsync();
        public async Task<Booking?> GetBookingByIdAsync(int id)
            => await repo.GetByIdAsync(id);
        public async Task<IEnumerable<Booking>> FindBookingsAsync(Expression<Func<Booking, bool>> condition)
            => await repo.FindAsync(condition);

        public async Task<ServiceResult> CreateBookingAsync(Booking booking)
        {
            var errors = CheckProperties(booking);

            if (errors.Any())
                return ServiceResult.Fail("Validation failed.", errors);

            await repo.AddAsync(booking);
            return ServiceResult.Ok("Booking was created successfully");
        }

        public async Task<ServiceResult> UpdateBookingAsync(Booking booking)
        {
            Booking? existing = await GetBookingByIdAsync(booking.Id);
            if (existing is null)
                return ServiceResult.Fail("Booking was not found.");

            var errors = CheckProperties(booking);

            if (errors.Any())
                return ServiceResult.Fail("Validation failed.", errors);

            await repo.UpdateAsync(booking);
            return ServiceResult.Ok("Booking was updated successfully");
        }

        public async Task<ServiceResult> DeleteBookingAsync(int bookingId)
        {
            Booking? booking = await GetBookingByIdAsync(bookingId);
            if (booking is null)
                return ServiceResult.Fail("Booking was not found");

            await repo.RemoveAsync(booking);
            return ServiceResult.Ok($"Booking was deleted successfully");
        }

        private static Dictionary<string, string> CheckProperties(Booking booking)
        {
            var errors = new Dictionary<string, string>();

            var priceError = ValidationHelper.ValidatePrice(booking.TotalPrice);
            if (priceError is not null)
                errors.Add("Total price", priceError);

            var timeError = ValidationHelper.ValidateTimeRange(booking.StartTime, booking.EndTime);
            if (timeError is not null)
                errors.Add("Time", timeError);

            return errors;
        }
    }
}
