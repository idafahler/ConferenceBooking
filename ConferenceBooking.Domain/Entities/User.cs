using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Domain.Entities
{
    public abstract class User
    {
        public int Id { get; init; }
        private string username;
        public string Username
        {
            get => username;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    username = value;
                    return;
                }
                username = value.Trim().ToLower();
            }
        }
        public string PasswordHash { get; set; }
        private string firstName;
        public string FirstName
        {
            get => firstName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    firstName = value;
                    return;
                }
                firstName = char.ToUpper(value.Trim()[0]) + value.Trim()[1..].ToLower();
            }
        }
        private string lastName;
        public string LastName
        {
            get => lastName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    lastName = value;
                    return;
                }
                lastName = char.ToUpper(value.Trim()[0]) + value.Trim()[1..].ToLower();
            }
        }
        private string email;
        public string Email
        {
            get => email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    email = value;
                    return;
                }
                email = value.Trim().ToLower();
            }
        }
        public List<Booking> Bookings { get; set; } = [];

        public string FullName => $"{FirstName} {LastName}";

        protected User(string username, string passwordHash, string firstName, string lastName, string email)
        {
            Username = username;
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        protected User() { }
    }
}
