using ConferenceBooking.Application.Interfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConferenceBooking.Application.Services
{
    public class RoomFeatureService(IRoomFeatureRepository featureRepo, IConferenceRoomRepository roomRepo) : IRoomFeatureService
    {
        public async Task<List<RoomFeature>> GetAllFeaturesAsync()
            => await featureRepo.GetAllAsync();
        public async Task<RoomFeature?> GetFeatureByIdAsync(int id)
            => await featureRepo.GetByIdAsync(id);
        public async Task<IEnumerable<RoomFeature>> FindFeaturesAsync(Expression<Func<RoomFeature, bool>> condition)
            => await featureRepo.FindAsync(condition);

        public async Task<ServiceResult> CreateFeatureAsync(RoomFeature feature)
        {
            var errors = new Dictionary<string, string>();
            var allFeatures = await GetAllFeaturesAsync();

            var nameError = ValidationHelper.ValidateUniqueName(feature.Name, "Name", allFeatures, f => f.Name);
            if (nameError is not null)
                errors.Add("Room feature", nameError);

            if (errors.Count != 0)
                return ServiceResult.Fail("Validation failed.", errors);

            await featureRepo.AddAsync(feature);
            return ServiceResult.Ok("Room feature was created successfully");
        }

        public async Task<ServiceResult> UpdateFeatureAsync(RoomFeature feature)
        {

            RoomFeature? existing = await GetFeatureByIdAsync(feature.Id);
            if (existing is null)
                return ServiceResult.Fail("Room feature was not found.");

            var allFeatures = (await GetAllFeaturesAsync())
                .Where(f => f.Id != feature.Id)
                .ToList();

            var errors = new Dictionary<string, string>();

            var nameError = ValidationHelper.ValidateUniqueName(feature.Name, "Name", allFeatures, f => f.Name);
            if (nameError is not null)
                errors.Add("Room feature", nameError);

            if (errors.Count != 0)
                return ServiceResult.Fail("Validation failed.", errors);

            await featureRepo.UpdateAsync(feature);
            return ServiceResult.Ok("Room feature was updated successfully");
        }

        public async Task<ServiceResult> DeleteFeatureAsync(int id)
        {
            RoomFeature? feature = await GetFeatureByIdAsync(id);
            if (feature is null)
                return ServiceResult.Fail("Room feature was not found.");

            var rooms = await roomRepo.GetAllRoomsWithFeaturesAsync();
            var hasLinks = rooms.Any(r => r.RoomFeatures.Any(f => f.Id == id));

            if (hasLinks)
                return ServiceResult.Fail($"{feature.Name} cannot be deleted because it is assigned to existing rooms.");

            await featureRepo.RemoveAsync(feature);
            return ServiceResult.Ok($"{feature.Name} was deleted successfully.");
        }
    }
}
