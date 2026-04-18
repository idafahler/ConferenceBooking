using ConferenceBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Domain.Entities
{
    public class Employee : User
    {
        public Department Department { get; set; }
        public Employee(string username, string passwordHash, string firstName, string lastName, string email, Department department)
            : base(username, passwordHash, firstName, lastName, email)
        {
            Department = department;
        }
        public Employee() { }
    }
}
