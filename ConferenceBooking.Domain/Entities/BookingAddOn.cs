using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Domain.Entities
{
    public class BookingAddOn
    {
        public int Id { get; init; }
        public int BookingId { get; set; }
        public Booking Booking { get; set; }
        public int AddOnId { get; set; }
        public AddOn AddOn { get; set; }
        public int Quantity { get; set; }
        public decimal BookedPricePerPerson { get; set; }
        public decimal TotalPrice => Quantity * BookedPricePerPerson;

        public BookingAddOn(int bookingId, int addOnId, int quantity, decimal bookedPricePerPerson)
        {
            BookingId = bookingId;
            AddOnId = addOnId;
            Quantity = quantity;
            BookedPricePerPerson = bookedPricePerPerson;
        }

        public BookingAddOn() { }
    }
}
