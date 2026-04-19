using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using ConferenceBooking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Infrastructure.Repositories
{
    public class ConferenceRoomRepository(ConferenceBookingContext context) : Repository<ConferenceRoom>(context), IConferenceRoomRepository
    {
        public async Task<List<ConferenceRoom>> GetAllRoomsWithFeaturesAsync()
            => await dbSet.Include(r => r.RoomFeatures).ToListAsync();
    }
}
