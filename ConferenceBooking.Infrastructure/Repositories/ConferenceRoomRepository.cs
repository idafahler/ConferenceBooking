using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ConferenceBooking.Application.RepositoryInterfaces;

namespace ConferenceBooking.Infrastructure.Repositories
{
    public class ConferenceRoomRepository(ConferenceBookingContext context) : Repository<ConferenceRoom>(context), IConferenceRoomRepository
    {
        public async Task<List<ConferenceRoom>> GetAllRoomsWithFeaturesAsync()
            => await dbSet.Include(r => r.RoomFeatures).ToListAsync();

        public async Task<ConferenceRoom?> GetRoomByIdWithFeaturesAsync(int id) 
            => await dbSet.Include(r => r.RoomFeatures).FirstOrDefaultAsync(r => r.Id == id);
    }
}
