using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Application.Factories
{
    public static class BookingFactory
    {
        public static Booking CreateBooking(int userId, int conferenceRoomId, DateTime startTime, DateTime endTime, decimal roomPrice, bool isFreeRoom)
        {
            var hours = (decimal)(endTime - startTime).TotalHours;

            return new Booking
            {
                UserId = userId,
                ConferenceRoomId = conferenceRoomId,
                StartTime = startTime,
                EndTime = endTime,
                TotalPrice = isFreeRoom ? 0 : hours * roomPrice, //if user is employee, then booking price is free
                CreatedAt = DateTime.Now
            };
        }

        public static BookingAddOn CreateBookingAddOnRelation(int bookingId, AddOn addOn, int quantity)
        {
            return new BookingAddOn
            {
                BookingId = bookingId,
                AddOnId = addOn.Id,
                Quantity = quantity,
                BookedPricePerPerson = addOn.PricePerPerson
            };
        }
    }
}
