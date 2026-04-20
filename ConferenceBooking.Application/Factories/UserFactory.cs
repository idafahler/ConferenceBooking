using ConferenceBooking.Application.Models;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Application.Factories
{
    public static class UserFactory
    {
        public static User CreateUser(UserDetails details)
        {
            return details.Role switch
            {
                UserType.Admin => new Admin(details.Username, details.Password, details.FirstName, details.LastName, details.Email),
                UserType.Employee => new Employee(details.Username, details.Password, details.FirstName, details.LastName, details.Email, details.Department!.Value),
                UserType.ExternalUser => new ExternalUser(details.Username, details.Password, details.FirstName, details.LastName, details.Email, details.Company!),
                _ => throw new ArgumentException($"Unknown user type.")
            };
        }
    }
}
