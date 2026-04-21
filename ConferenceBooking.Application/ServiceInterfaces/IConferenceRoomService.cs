using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConferenceBooking.Application.ServiceInterfaces
{
    public interface IConferenceRoomService
    {
        Task<List<ConferenceRoom>> GetAllRoomsAsync();
        Task<ConferenceRoom?> GetRoomByIdAsync(int id);
        Task<IEnumerable<ConferenceRoom>> FindRoomsAsync(Expression<Func<ConferenceRoom, bool>> condition);
        Task<ConferenceRoom?> GetRoomByIdWithFeaturesAsync(int id);
        Task<ServiceResult> CreateRoomAsync(ConferenceRoom room);
        Task<ServiceResult> UpdateRoomAsync(ConferenceRoom room);
        Task<ServiceResult> DeleteRoomAsync(int id);
    }
}
