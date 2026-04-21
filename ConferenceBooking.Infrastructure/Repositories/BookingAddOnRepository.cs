using ConferenceBooking.Application.RepositoryInterfaces;
using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Infrastructure.Repositories
{
    public class BookingAddOnRepository(ConferenceBookingContext context) : Repository<BookingAddOn>(context), IBookingAddOnRepository
    {
    }
}
