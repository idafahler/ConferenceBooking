using ConferenceBooking.Application.Interfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConferenceBooking.Application.Services
{
    public class AddOnService(IAddOnRepository repo, IBookingAddOnRepository bookingAddOnRepo) : IAddOnService
    {
        public async Task<List<AddOn>> GetAllAddOnsAsync() 
            => await repo.GetAllAsync();
        public async Task<AddOn?> GetAddOnByIdAsync(int id) 
            => await repo.GetByIdAsync(id);
        public async Task<IEnumerable<AddOn>> FindAddOnsAsync(Expression<Func<AddOn, bool>> condition)
            => await repo.FindAsync(condition);

        public async Task<ServiceResult> CreateAddOnAsync(AddOn addOn)
        {
            var allAddOns = await GetAllAddOnsAsync();

            var errors = CheckProperties(addOn, allAddOns);

            if (errors.Any())
                return ServiceResult.Fail("Validation failed.", errors);

            await repo.AddAsync(addOn);
            return ServiceResult.Ok("Add on was created successfully");
        }

        public async Task<ServiceResult> UpdateAddOnAsync(AddOn addOn)
        {
            AddOn? existing = await GetAddOnByIdAsync(addOn.Id);
            if (existing is null)
                return ServiceResult.Fail("Add on was not found.");

            var allAddOns = (await GetAllAddOnsAsync())
                .Where(a => a.Id != addOn.Id) //sorting out itself
                .ToList();

            var errors = CheckProperties(addOn, allAddOns);

            if (errors.Any())
                return ServiceResult.Fail("Validation failed.", errors);

            await repo.UpdateAsync(addOn);
            return ServiceResult.Ok("Addon was updated successfully.");
        }

        public async Task<ServiceResult> DeleteAddOnAsync(int id)
        {
            var addOn = await GetAddOnByIdAsync(id);
            if (addOn is null)
                return ServiceResult.Fail("Add on was not found");

            var bookingAddOns = await bookingAddOnRepo.FindAsync(ba => ba.AddOnId == id);
            if (bookingAddOns.Any())
                return ServiceResult.Fail($"{addOn.Name} can't be deleted because it is linked to existing bookings.");

            await repo.RemoveAsync(addOn);
            return ServiceResult.Ok($"{addOn.Name} was deleted successfully");
        }

        private static Dictionary<string, string> CheckProperties(AddOn addOn, List<AddOn> allAddOns)
        {
            var errors = new Dictionary<string, string>();

            var nameError = ValidationHelper.ValidateUniqueName(addOn.Name, "Name",allAddOns, a => a.Name);
            if (nameError is not null)
                errors.Add("Name", nameError);

            var priceError = ValidationHelper.ValidatePrice(addOn.PricePerPerson);
            if (priceError is not null)
                errors.Add("Price per person", priceError);

            return errors;
        }
    }
}
