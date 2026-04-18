using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Domain.Entities
{
    public class ConferenceRoom
    {
        public int Id { get; init; }
        private string number;
        public string Number
        {
            get => number;
            set => number = value.Trim();
        }
        public int Capacity { get; set; }
        public decimal PricePerHour { get; set; }
        public List<RoomFeature> RoomFeatures { get; set; } = [];
        public List<Booking> Bookings { get; set; } = [];
        public ConferenceRoom(string number, int capacity, decimal pricePerHour)
        {
            Number = number;
            Capacity = capacity;
            PricePerHour = pricePerHour;
        }
        public ConferenceRoom() { }
    }
}
