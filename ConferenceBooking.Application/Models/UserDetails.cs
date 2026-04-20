using ConferenceBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Application.Models
{
    public class UserDetails(UserType role, string username, string password, string firstName, string lastName, string email, Department? department = null, string? company = null)
    {
        public UserType Role { get; set; } = role;
        public string Username { get; set; } = username;
        public string Password { get; set; } = password;
        public string FirstName { get; set; } = firstName;
        public string LastName { get; set; } = lastName;
        public string Email { get; set; } = email;
        public Department? Department { get; set; } = department;
        public string? Company { get; set; } = company;
    }
}
