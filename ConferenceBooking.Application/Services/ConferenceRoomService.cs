using ConferenceBooking.Application.RepositoryInterfaces;
using ConferenceBooking.Application.ServiceInterfaces;
using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConferenceBooking.Application.Services
{
    public class ConferenceRoomService(IConferenceRoomRepository roomRepo, IBookingRepository bookingRepo) : IConferenceRoomService
    {
        public async Task<List<ConferenceRoom>> GetAllRoomsAsync()
            => await roomRepo.GetAllAsync();
        public async Task<ConferenceRoom?> GetRoomByIdAsync(int id)
            => await roomRepo.GetByIdAsync(id);
        public async Task<IEnumerable<ConferenceRoom>> FindRoomsAsync(Expression<Func<ConferenceRoom, bool>> condition)
            => await roomRepo.FindAsync(condition);
        public async Task<ConferenceRoom?> GetRoomByIdWithFeaturesAsync(int id) 
            => await roomRepo.GetRoomByIdWithFeaturesAsync(id);

        public async Task<ServiceResult> CreateRoomAsync(ConferenceRoom room)
        {
            var allRooms = await GetAllRoomsAsync();

            var maxError = ValidationHelper.ValidateAmountInstances(allRooms, "conference rooms"); //cannot be over 9 conference rooms
            if (maxError is not null)
                return ServiceResult.Fail(maxError);

            var errors = CheckProperties(room, allRooms);

            if (errors.Count != 0)
                return ServiceResult.Fail("Validation failed.", errors);

            await roomRepo.AddAsync(room);
            return ServiceResult.Ok("Conference room was created successfully");
        }

        public async Task<ServiceResult> UpdateRoomAsync(ConferenceRoom roomIn)
        {
            ConferenceRoom? room = await GetRoomByIdAsync(roomIn.Id);
            if (room is null)
                return ServiceResult.Fail("Conference room was not found.");

            var allRooms = (await GetAllRoomsAsync()) //gets all rooms except for this room
                .Where(r => r.Id != room.Id)
                .ToList();

            var errors = CheckProperties(roomIn, allRooms);

            if (errors.Count != 0)
                return ServiceResult.Fail("Validation failed.", errors);

            await roomRepo.UpdateAsync(roomIn);
            return ServiceResult.Ok("Conference room was updated successfully");
        }

        public async Task<ServiceResult> DeleteRoomAsync(int roomId)
        {
            ConferenceRoom? room = await GetRoomByIdAsync(roomId);
            if (room is null)
                return ServiceResult.Fail("Room was not found");

            var bookings = await bookingRepo.FindAsync(b => b.ConferenceRoomId == roomId);
            if (bookings.Any())//if there is any bookings connected to conference room. conference room cannot be deleted
                return ServiceResult.Fail($"{room.Number} cannot be deleted because it has existing bookings.");

            await roomRepo.RemoveAsync(room);
            return ServiceResult.Ok($"Room was deleted successfully");
        }

        private static Dictionary<string, string> CheckProperties(ConferenceRoom room, List<ConferenceRoom> allRooms)
        {
            var errors = new Dictionary<string, string>();

            var nameError = ValidationHelper.ValidateUniqueName(room.Number,"Number" ,allRooms, c => c.Number);
            if (nameError is not null)
                errors.Add("Number", nameError);

            if (room.Number != null && room.Number.Length > 4)
                errors.Add("Number length", "Room number cannot exceed 4 characters.");

            var capacityError = ValidationHelper.ValidateCapacity(room.Capacity);
            if(capacityError is not null)
                errors.Add("Capacity", capacityError);

            var priceError = ValidationHelper.ValidatePrice(room.PricePerHour);
            if (priceError is not null)
                errors.Add("Price per hour", priceError);

            return errors;
        }
    }
}
