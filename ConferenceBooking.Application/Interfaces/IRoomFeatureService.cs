using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConferenceBooking.Application.Interfaces
{
    public interface IRoomFeatureService
    {
        Task<List<RoomFeature>> GetAllFeaturesAsync();
        Task<RoomFeature?> GetFeatureByIdAsync(int id);
        Task<IEnumerable<RoomFeature>> FindFeaturesAsync(Expression<Func<RoomFeature, bool>> condition);
        Task<ServiceResult> CreateFeatureAsync(RoomFeature feature);
        Task<ServiceResult> UpdateFeatureAsync(RoomFeature feature);
        Task<ServiceResult> DeleteFeatureAsync(int id);
    }
}
