using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using ConferenceBooking.Application.RepositoryInterfaces;

namespace ConferenceBooking.Infrastructure.Repositories
{
    public class UserRepository(ConferenceBookingContext context) : Repository<User>(context), IUserRepository
    {
    }
}
