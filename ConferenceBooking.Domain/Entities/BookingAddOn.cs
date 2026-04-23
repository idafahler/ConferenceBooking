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
        public int Quantity { get; set; } //how much capacity in conference room
        public decimal BookedPricePerPerson { get; set; } //saves cost for add on at creation of booking, if it where to change history of pricing is still correct
        public decimal TotalPrice => Quantity * BookedPricePerPerson; //computed property for easily getting totalprice out of add on.

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
