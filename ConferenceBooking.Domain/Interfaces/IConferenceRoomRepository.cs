using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Domain.Interfaces
{
    public interface IConferenceRoomRepository : IRepository<ConferenceRoom>
    {
        Task<List<ConferenceRoom>> GetRoomsWithFeaturesAsync();
    }
}
