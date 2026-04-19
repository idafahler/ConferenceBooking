using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using ConferenceBooking.Domain.Interfaces;

namespace ConferenceBooking.Infrastructure.Repositories
{
    public class AddOnRepository(ConferenceBookingContext context) : Repository<AddOn>(context), IAddOnRepository
    {
    }
}
