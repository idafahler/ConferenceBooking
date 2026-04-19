using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConferenceBooking.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> FindUsersAsync(Expression<Func<User, bool>> condition);
        Task<ServiceResult> CreateUserAsync(User user);
        Task<ServiceResult> UpdateUserAsync(User user);
        Task<ServiceResult> DeleteUserAsync(int id);
    }
}
