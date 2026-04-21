using ConferenceBooking.Application.Factories;
using ConferenceBooking.Application.RepositoryInterfaces;
using ConferenceBooking.Application.ServiceInterfaces;
using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConferenceBooking.Application.Services
{
    public class BookingService(IBookingRepository repo, IConferenceRoomRepository roomRepo, IAddOnRepository addOnRepo,
        IBookingAddOnRepository bookingAddOnRepo) : IBookingService
    {
        public async Task<List<Booking>> GetAllBookingsAsync()
            => await repo.GetAllAsync();
        public async Task<Booking?> GetBookingByIdAsync(int id)
            => await repo.GetByIdAsync(id);
        public async Task<IEnumerable<Booking>> FindBookingsAsync(Expression<Func<Booking, bool>> condition)
            => await repo.FindAsync(condition);

        public async Task<List<Booking>> GetAllBookingsForUserWithDetailsAsync(int userId)
            => await repo.GetAllBookingsForUserWithDetailsAsync(userId);

        public async Task<Booking?> GetBookingByIdWithDetailsAsync(int bookingId)
            => await repo.GetBookingByIdWithDetailsAsync(bookingId);

        public async Task<ServiceResult> CreateBookingAsync(User user, int roomId, DateTime start, DateTime end, List<int> addOnIds)
        {
            var room = await roomRepo.GetByIdAsync(roomId);
            if (room is null)
                return ServiceResult.Fail("Room not found.");

            var errors = new Dictionary<string, string>();

            var timeError = ValidationHelper.ValidateTimeRange(start, end);
            if (timeError is not null)
                errors.Add("Time", timeError);

            var allBookings = await GetAllBookingsAsync();

            var conflictbooking = ValidationHelper.ValidateNoBookingConflict
                (start, end, roomId, allBookings);
            if (conflictbooking is not null)
                errors.Add("Conflict", conflictbooking);

            if (errors.Count != 0)
                return ServiceResult.Fail("Validation failed.", errors);

            var booking = BookingFactory.CreateBooking(user.Id, roomId, start, end, room.PricePerHour, user is Employee);

            await repo.AddAsync(booking);

            decimal addOnTotal = 0;
            foreach(var id in addOnIds)
            {
                var addOn = await addOnRepo.GetByIdAsync(id);
                if (addOn is null) continue;

                var bookingAddOn = BookingFactory.CreateAddOn(booking.Id, addOn, room.Capacity);
                await bookingAddOnRepo.AddAsync(bookingAddOn);
                addOnTotal += bookingAddOn.TotalPrice;
            }

            if (addOnTotal > 0)
            {
                booking.TotalPrice += addOnTotal;
                await repo.UpdateAsync(booking);
            }

            return ServiceResult.Ok("Booking was created successfully");
        }

        public async Task<ServiceResult> UpdateBookingAsync(Booking booking)
        {
            Booking? existing = await GetBookingByIdAsync(booking.Id);
            if (existing is null)
                return ServiceResult.Fail("Booking was not found.");

            var errors = new Dictionary<string, string>();

            var timeError = ValidationHelper.ValidateTimeRange(booking.StartTime, booking.EndTime);
            if (timeError is not null)
                errors.Add("Time", timeError);

            var allBookings = await GetAllBookingsAsync();

            var conflictError = ValidationHelper.ValidateNoBookingConflict
                (booking.StartTime, booking.EndTime,booking.ConferenceRoomId ,allBookings , booking.Id);
            if (conflictError is not null)
                errors.Add("Conflict", conflictError);

            if (errors.Count != 0)
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
    }
}
