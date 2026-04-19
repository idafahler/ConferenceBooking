using ConferenceBooking.Application.Interfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ConferenceBooking.Application.Services
{
    public class UserService(IUserRepository repo) : IUserService
    {
        public async Task<List<User>> GetAllUsersAsync()
            => await repo.GetAllAsync();
        public async Task<User?> GetUserByIdAsync(int id)
            => await repo.GetByIdAsync(id);
        public async Task<IEnumerable<User>> FindUsersAsync(Expression<Func<User, bool>> condition)
            => await repo.FindAsync(condition);


    }
}
