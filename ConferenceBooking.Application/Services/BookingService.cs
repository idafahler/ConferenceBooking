using ConferenceBooking.Application.Factories;
using ConferenceBooking.Application.RepositoryInterfaces;
using ConferenceBooking.Application.ServiceInterfaces;
using ConferenceBooking.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConferenceBooking.Application.Services
{
    public class BookingService(IBookingRepository repo, IConferenceRoomRepository roomRepo, IAddOnRepository addOnRepo,
        IBookingAddOnRepository bookingAddOnRepo, IUserRepository userRepo) : IBookingService
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

            var allBookingsSameDay = await FindBookingsAsync(b => b.StartTime.Date == start.Date);
            List<Booking> allBookingsSameDayList = allBookingsSameDay.ToList();

            var conflictBooking = ValidationHelper.ValidateNoBookingConflict
                (start, end, roomId, allBookingsSameDayList);
            if (conflictBooking is not null)
                errors.Add("Conflict", conflictBooking);

            if (errors.Count != 0)
                return ServiceResult.Fail("Validation failed.", errors);

            var booking = BookingFactory.CreateBooking(user.Id, roomId, start, end, room.PricePerHour, user is Employee);

            await repo.AddAsync(booking);

            decimal addOnTotal = 0;
            foreach(var id in addOnIds)
            {
                var addOn = await addOnRepo.GetByIdAsync(id);
                if (addOn is null) continue;

                var bookingAddOn = BookingFactory.CreateBookingAddOnRelation(booking.Id, addOn, room.Capacity);
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

            var allBookingsSameDay = await FindBookingsAsync(b => b.StartTime.Date == booking.StartTime.Date);
            List<Booking> allBookingsSameDayList = allBookingsSameDay.ToList();

            var conflictError = ValidationHelper.ValidateNoBookingConflict(
                booking.StartTime, booking.EndTime, booking.ConferenceRoomId, allBookingsSameDayList, booking.Id);
            if (conflictError is not null)
                errors.Add("Conflict", conflictError);

            if (errors.Count != 0)
                return ServiceResult.Fail("Validation failed.", errors);

            var room = await roomRepo.GetByIdAsync(booking.ConferenceRoomId);
            var user = await userRepo.GetByIdAsync(booking.UserId!.Value);
            if (user is null || room is null)
                return ServiceResult.Fail("Not found.");

            var hours = (decimal)(booking.EndTime - booking.StartTime).TotalHours;
            var roomPrice = user is Employee ? 0 : hours * room!.PricePerHour;
            var addonTotal = await bookingAddOnRepo.CalculateTotalAddonPriceAsync(booking.Id);
            booking.TotalPrice = roomPrice + addonTotal;

            await repo.UpdateAsync(booking);
            return ServiceResult.Ok("Booking was updated successfully.");
        }

        public async Task<ServiceResult> DeleteBookingAsync(int bookingId)
        {
            Booking? booking = await GetBookingByIdAsync(bookingId);
            if (booking is null)
                return ServiceResult.Fail("Booking was not found");

            await repo.RemoveAsync(booking);
            return ServiceResult.Ok($"Booking was deleted successfully");
        }

        public async Task<ServiceResult> AddAddonToBookingAsync(int bookingId, int addonId)
        {
            var booking = await repo.GetByIdAsync(bookingId);
            if (booking is null)
                return ServiceResult.Fail("Booking not found.");

            var addon = await addOnRepo.GetByIdAsync(addonId);
            if (addon is null)
                return ServiceResult.Fail("Add on not found.");

            var existingAddons = await bookingAddOnRepo.FindAsync(ba => ba.BookingId == bookingId && ba.AddOnId == addonId);
            if (existingAddons.Any())
                return ServiceResult.Fail($"{addon.Name} is already added to this booking.");

            var room = await roomRepo.GetByIdAsync(booking.ConferenceRoomId);
            if (room is null)
                return ServiceResult.Fail("Room not found.");

            var bookingAddon = BookingFactory.CreateBookingAddOnRelation(bookingId, addon, room.Capacity);
            await bookingAddOnRepo.AddAsync(bookingAddon);

            booking.TotalPrice += bookingAddon.TotalPrice;
            await repo.UpdateAsync(booking);

            return ServiceResult.Ok($"{addon.Name} added to booking.");
        }

        public async Task<ServiceResult> RemoveAddonFromBookingAsync(int bookingId, int bookingAddonId)
        {
            var booking = await repo.GetByIdAsync(bookingId);
            if (booking is null)
                return ServiceResult.Fail("Booking not found.");

            var bookingAddon = await bookingAddOnRepo.GetByIdAsync(bookingAddonId);
            if (bookingAddon is null)
                return ServiceResult.Fail("Add on not found on booking.");

            booking.TotalPrice -= bookingAddon.TotalPrice;
            await bookingAddOnRepo.RemoveAsync(bookingAddon);
            await repo.UpdateAsync(booking);

            return ServiceResult.Ok("Add on removed from booking.");
        }
    }
}
