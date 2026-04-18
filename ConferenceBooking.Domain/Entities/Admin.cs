using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Domain.Entities
{
    public class Admin : User
    {
        public Admin(string username, string passwordHash, string firstName, string lastName, string email)
            : base(username, passwordHash, firstName, lastName, email) { }
        public Admin() { }
    }
}
