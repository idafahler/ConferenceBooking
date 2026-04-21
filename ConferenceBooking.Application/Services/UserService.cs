using ConferenceBooking.Application.Factories;
using ConferenceBooking.Application.Models;
using ConferenceBooking.Application.RepositoryInterfaces;
using ConferenceBooking.Application.ServiceInterfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using static BCrypt.Net.BCrypt;

namespace ConferenceBooking.Application.Services
{
    public class UserService(IUserRepository userRepo, IBookingRepository bookingRepo) : IUserService
    {
        public async Task<List<User>> GetAllUsersAsync()
            => await userRepo.GetAllAsync();
        public async Task<User?> GetUserByIdAsync(int id)
            => await userRepo.GetByIdAsync(id);
        public async Task<IEnumerable<User>> FindUsersAsync(Expression<Func<User, bool>> condition)
            => await userRepo.FindAsync(condition);

        public async Task<ServiceResult> CreateUserAsync(UserDetails details)
        {
            var errors = new Dictionary<string, string>();

            var allUsers = await GetAllUsersAsync();
            var usernameError = ValidationHelper.ValidateUniqueName(details.Username, "Username", allUsers, u => u.Username);
            if (usernameError is not null)
                errors.Add("Username", usernameError);

            var passwordError = ValidationHelper.ValidatePassWord(details.Password);
            if (passwordError is not null)
                errors.Add("Password", passwordError);

            var firstNameError = ValidationHelper.ValidateName(details.FirstName, "First name");
            if (firstNameError is not null)
                errors.Add("First name", firstNameError);

            var lastNameError = ValidationHelper.ValidateName(details.LastName, "Last name");
            if (lastNameError is not null)
                errors.Add("Last name", lastNameError);

            var emailError = ValidationHelper.ValidateEmail(details.Email);
            if (emailError is not null)
                errors.Add("Email", emailError);
            else
            {
                var uniqueEmail = ValidationHelper.ValidateUniqueName(details.Email, "Email", allUsers, u => u.Email);
                if (uniqueEmail is not null)
                    errors.Add("Email", uniqueEmail);
            }

            if (details.Role == UserType.Employee && details.Department is null)
            {
                 errors.Add("Department", "Department is required for employees.");
            }

            if (details.Role == UserType.ExternalUser)
            {
                var companyError = ValidationHelper.ValidateName(details.Company, "Company name");
                if (companyError is not null)
                    errors.Add("Company", companyError);
            }

            if (errors.Count != 0)
                return ServiceResult.Fail("Validation failed.", errors);

            var user = UserFactory.CreateUser(details);
            user.PasswordHash = HashPassword(details.Password);
            await userRepo.AddAsync(user);
            return ServiceResult.Ok("User was created successfully.");
        }

        public async Task<ServiceResult> UpdateUserAsync(User userIn)
        {
            User? existing = await GetUserByIdAsync(userIn.Id);
            if (existing is null)
                return ServiceResult.Fail("User was not found.");

            var listOfUsers = (await GetAllUsersAsync())
                .Where(u => u.Id != userIn.Id)
                .ToList();

            var errors = new Dictionary<string, string>();

            var usernameError = ValidationHelper.ValidateUniqueName(userIn.Username, "Username", listOfUsers, u => u.Username);
            if (usernameError is not null)
                errors.Add("Username", usernameError);

            var firstNameError = ValidationHelper.ValidateName(userIn.FirstName, "First name");
            if (firstNameError is not null)
                errors.Add("First name", firstNameError);

            var lastNameError = ValidationHelper.ValidateName(userIn.LastName, "Last name");
            if (lastNameError is not null)
                errors.Add("Last name", lastNameError);

            var emailError = ValidationHelper.ValidateEmail(userIn.Email);
            if (emailError is not null)
                errors.Add("Email", emailError);
            else
            {
                var uniqueEmail = ValidationHelper.ValidateUniqueName(userIn.Email, "Email", listOfUsers, u => u.Email);
                if (uniqueEmail is not null)
                    errors.Add("Email", uniqueEmail);
            }

            if (userIn is Employee employee)
            {
                if (!Enum.IsDefined(employee.Department))
                    errors.Add("Department", "Invalid department");
            }

            if (userIn is ExternalUser external)
            {
                var companyError = ValidationHelper.ValidateName(external.Company, "Company name");
                if (companyError is not null)
                    errors.Add("Company", companyError);
            }

            if (errors.Count != 0)
                return ServiceResult.Fail("Validation failed.", errors);

            await userRepo.UpdateAsync(userIn);
            return ServiceResult.Ok("User was updated successfully.");
        }

        public async Task<ServiceResult> ChangePasswordAsync(User userIn, string newPassword)
        {
            var user = await GetUserByIdAsync(userIn.Id);
            if (user is null)
                return ServiceResult.Fail("User was not found.");

            var passwordError = ValidationHelper.ValidatePassWord(newPassword);
            if (passwordError is not null)
                return ServiceResult.Fail(passwordError);

            user.PasswordHash = HashPassword(newPassword);
            await userRepo.UpdateAsync(user);
            return ServiceResult.Ok("Password was updated successfully.");
        }

        public async Task<ServiceResult> DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user is null)
                return ServiceResult.Fail("User was not found.");

            var bookings = await bookingRepo.FindAsync(b => b.UserId == id);
            if (bookings.Any())
                return ServiceResult.Fail($"{user.FullName} cannot be deleted because user has bookings.");

            await userRepo.RemoveAsync(user);
            return ServiceResult.Ok($"{user.FullName} was deleted successfully.");
        }



        public async Task<User?> GetByUsernameAsync(string username)
        {
            var users = await FindUsersAsync(u => u.Username == username);
            return users.FirstOrDefault();
        }

        public async Task<ServiceResult<User>> AuthenticateUserAsync(string username, string password)
        {
            var user = await GetByUsernameAsync(username);

            if (user is null || !Verify(password, user.PasswordHash))
                return ServiceResult<User>.Fail("Invalid credentials");

            return ServiceResult<User>.Ok(user, $"Welcome {user.FullName}");
        }
    }
}
