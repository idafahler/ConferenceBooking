using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using ConferenceBooking.Application.RepositoryInterfaces;

namespace ConferenceBooking.Infrastructure.Repositories
{
    public class RoomFeatureRepository(ConferenceBookingContext context) : Repository<RoomFeature>(context), IRoomFeatureRepository
    {
    }
}
