using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Application.RepositoryInterfaces
{
    public interface IConferenceRoomRepository : IRepository<ConferenceRoom>
    {
        Task<List<ConferenceRoom>> GetAllRoomsWithFeaturesAsync();
        Task<ConferenceRoom?> GetRoomByIdWithFeaturesAsync(int id);
    }
}
