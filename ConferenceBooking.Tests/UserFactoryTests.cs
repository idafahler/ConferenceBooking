using ConferenceBooking.Application.Factories;
using ConferenceBooking.Application.Models;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Enums;

namespace ConferenceBooking.Tests
{
    public class UserFactoryTests
    {
        [Fact]
        public void CreateUser_Admin_ReturnsAdmin()
        {
            var details = new UserDetails(UserType.Admin,
                "admin123", "Admin123!", "Anna", "Andersson", "anna.andersson@mail.se");

            var user = UserFactory.CreateUser(details);

            Assert.IsType<Admin>(user);
        }

        [Fact]
        public void CreateUser_Employee_ReturnsEmployeeWithDepartment()
        {
            var details = new UserDetails(UserType.Employee,
                "lollo.legal", "Lollo123!", "Lollo", "Ek", "lollo.ek@mail.se", Department.Legal);

            var user = UserFactory.CreateUser(details);

            Assert.IsType<Employee>(user);
            Assert.Equal(Department.Legal, ((Employee)user).Department);
        }

        [Fact]
        public void CreateUser_ExternalUser_ReturnsExternalWithCompany()
        {
            var details = new UserDetails(UserType.ExternalUser,
                "guest", "Guest123!", "Guest", "Guestsson", "guest.guestsson@mail.com", company: "Åhlens");

            var user = UserFactory.CreateUser(details);

            Assert.IsType<ExternalUser>(user);
            Assert.Equal("Åhlens", ((ExternalUser)user).Company);
        }
    }
}
