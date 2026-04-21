using ConferenceBooking.Application.Models;
using ConferenceBooking.Application.ServiceInterfaces;
using ConferenceBooking.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Infrastructure
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceScopeFactory scopeFactory)
        {
            using var scope = scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            var existingUsers = await userService.GetAllUsersAsync();

            if (existingUsers.Count != 0)
                return;

            var users = new List<UserDetails>
            {
                new(UserType.Admin, "admin", "Admin123!", "Admin", "Adminsson", "admin@conference.com"),
                new(UserType.Employee, "erik.it", "Erik123!", "Erik", "Eriksson", "erik@conference.com", Department.IT),
                new(UserType.Employee, "maria.hr", "Maria123!", "Maria", "Johansson", "maria@conference.com", Department.HR),
                new(UserType.ExternalUser, "karlvolvo", "Karl123!", "Karl", "Svensson", "karl@volvo.com", company: "Volvo"),
                new(UserType.ExternalUser, "lisaikea", "Lisa123!", "Lisa", "Nilsson", "lisa@ikea.com", company: "IKEA")
            };

            foreach(var userDetails in users)
            {
                await userService.CreateUserAsync(userDetails);
            }
        }
    }
}
