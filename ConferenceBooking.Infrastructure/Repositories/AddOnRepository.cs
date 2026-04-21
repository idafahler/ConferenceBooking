using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using ConferenceBooking.Application.RepositoryInterfaces;

namespace ConferenceBooking.Infrastructure.Repositories
{
    public class AddOnRepository(ConferenceBookingContext context) : Repository<AddOn>(context), IAddOnRepository
    {
    }
}
