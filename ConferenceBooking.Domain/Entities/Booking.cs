using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Domain.Entities
{
    public class Booking
    {
        public int Id { get; init; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public int ConferenceRoomId { get; set; }
        public ConferenceRoom ConferenceRoom { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; init; }
        public List<BookingAddOn> BookingAddOns { get; set; } = [];

        public Booking(int userId, int conferenceRoomId, DateTime startTime, DateTime endTime, decimal totalPrice, DateTime createdAt)
        {
            UserId = userId;
            ConferenceRoomId = conferenceRoomId;
            StartTime = startTime;
            EndTime = endTime;
            TotalPrice = totalPrice;
            CreatedAt = createdAt;
        }
        public Booking() { }
    }
}
