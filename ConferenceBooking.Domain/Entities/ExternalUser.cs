using ConferenceBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Domain.Entities
{
    public class ExternalUser : User
    {
        private string company;
        public string Company
        {
            get => company;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    company = value;
                    return;
                }
                company = char.ToUpper(value.Trim()[0]) + value.Trim()[1..].ToLower();
            }
        }
        public ExternalUser(string username, string passwordHash, string firstName, string lastName, string email, string company)
            : base(username, passwordHash, firstName, lastName, email)
        {
            Company = company;
        }
        public ExternalUser() { }
    }
}
