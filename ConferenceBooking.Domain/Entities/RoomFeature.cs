using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Domain.Entities
{
    public class RoomFeature
    {
        public int Id { get; init; }
        private string name;
        public string Name
        {
            get => name;
            set => name = char.ToUpper(value.Trim()[0]) + value.Trim()[1..].ToLower();
        }
        public List<ConferenceRoom> ConferenceRooms { get; set; } = [];

        public RoomFeature(string name)
        {
            Name = name;
        }
        public RoomFeature() { }
    }
}
