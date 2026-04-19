using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConferenceBooking.Application.Interfaces
{
    public interface IAddOnService
    {
        Task<List<AddOn>> GetAllAddOnsAsync();
        Task<AddOn?> GetAddOnByIdAsync(int id);
        Task<IEnumerable<AddOn>> FindAddOnsAsync(Expression<Func<AddOn, bool>> condition);
        Task<ServiceResult> CreateAddOnAsync(AddOn addOn);
        Task<ServiceResult> UpdateAddOnAsync(AddOn addOn);
        Task<ServiceResult> DeleteAddOnAsync(int id);
    }
}
