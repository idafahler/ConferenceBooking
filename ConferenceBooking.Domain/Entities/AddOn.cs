using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Domain.Entities
{
    public class AddOn
    {
        public int Id { get; init; }
        private string name;
        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    name = value;
                    return;
                }
                name = char.ToUpper(value.Trim()[0]) + value.Trim()[1..].ToLower();
            }
        }
        public decimal PricePerPerson { get; set; }
        public List<BookingAddOn> BookingAddOns { get; set; } = [];

        public AddOn(string name, decimal pricePerPerson)
        {
            Name = name;
            PricePerPerson = pricePerPerson;
        }

        public AddOn() { }
    }
}
