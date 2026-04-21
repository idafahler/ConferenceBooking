using ConferenceBooking.Application;
using ConferenceBooking.Application.Models;
using ConferenceBooking.Application.ServiceInterfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ConferenceBooking.Presentation.Shared
{
    internal class SharedOperations(IServiceScopeFactory scopeFactory)
    {
        internal async Task CreateNewUser(UserType role)
        {
            Console.Write("Username: ");
            var username = Console.ReadLine()!;
            Console.Write("Password: ");
            var password = SharedUIMethods.ReadPassword();
            Console.Write("First name: ");
            var firstName = Console.ReadLine()!;
            Console.Write("Last name: ");
            var lastName = Console.ReadLine()!;
            Console.Write("Email: ");
            var email = Console.ReadLine()!;

            Department? department = null;
            string? company = null;

            if (role == UserType.Employee)
            {
                var departments = string.Join(", ", Enum.GetValues<Department>());
                Console.WriteLine($"All departments: {departments}");
                Console.Write("\nDepartment: ");
                department = Enum.Parse<Department>(Console.ReadLine()!, true);
            }
            else if (role == UserType.ExternalUser)
            {
                Console.Write("Company: ");
                company = Console.ReadLine();
            }

            var details = new UserDetails(role, username, password, firstName, lastName, email, department, company);

            var fixers = new Dictionary<string, Action>
            {
                ["Username"] = () =>
                {
                    Console.Write("Username: ");
                    details.Username = Console.ReadLine()!;
                },

                ["Password"] = () =>
                {
                    Console.Write("Password: ");
                    details.Password = SharedUIMethods.ReadPassword();
                },

                ["First name"] = () =>
                {
                    Console.Write("First name: ");
                    details.FirstName = Console.ReadLine()!;
                },

                ["Last name"] = () =>
                {
                    Console.Write("Last name: ");
                    details.LastName = Console.ReadLine()!;
                },

                ["Email"] = () =>
                {
                    Console.Write("Email: ");
                    details.Email = Console.ReadLine()!;
                },

                ["Department"] = () =>
                {
                    Console.Write("Department: ");
                    details.Department = Enum.Parse<Department>(Console.ReadLine()!, true);
                },

                ["Company"] = () =>
                {
                    Console.Write("Company: ");
                    details.Company = Console.ReadLine();
                }
            };

            ServiceResult result;
            do
            {
                using var scope = scopeFactory.CreateScope();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                result = await userService.CreateUserAsync(details);

                if (!result.Success)
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"{error.Value}. Try again.");
                        if (fixers.TryGetValue(error.Key, out var fixer))
                            fixer();
                    }
                }
            } while (!result.Success);

            SharedUIMethods.PrintResultMessage(result);
        }

        internal async Task UpdateUserDetails(int userId)
        {
            while (true)
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IUserService>();
                var user = await service.GetUserByIdAsync(userId);

                var options = new List<string>
                {
                    $"Username: {user!.Username}",
                    $"Email: {user.Email}",
                    $"First name: {user.FirstName}",
                    $"Last name: {user.LastName}",
                    "Password"
                };
                if (user is Employee emp)
                    options.Add($"Department: {emp.Department}");
                if (user is ExternalUser external)
                    options.Add($"Company: {external.Company}");

                Console.Clear();
                var key = Menu.Show("Managing account", "Back", options.ToArray());


                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        await UpdateProperty(user.Id, "username", (u, value) => u.Username = value);
                        break;
                    case ConsoleKey.D2:
                        await UpdateProperty(user.Id, "email", (u, value) => u.Email = value);
                        break;
                    case ConsoleKey.D3:
                        await UpdateProperty(user.Id, "first name", (u, value) => u.FirstName = value);
                        break;
                    case ConsoleKey.D4:
                        await UpdateProperty(user.Id, "last name", (u, value) => u.LastName = value);
                        break;
                    case ConsoleKey.D5:
                        await UpdatePassword(user.Id);
                        break;
                    case ConsoleKey.D6:
                        if (user is Employee)
                            await UpdateDepartment(user.Id);
                        else if (user is ExternalUser)
                            await UpdateProperty(user.Id, "company", (u, value) => ((ExternalUser)u).Company = value);
                        break;
                    case ConsoleKey.D0:
                        return;
                    default:
                        SharedUIMethods.PrintMessageSleep("Invalid choice.");
                        break;
                }
            }
        }

        internal async Task UpdateProperty(int userId, string property, Action<User, string> update)
        {
            while (true)
            {
                Console.Write($"Enter new {property} (0 to cancel): ");
                var input = Console.ReadLine()!;
                if (input == "0") return;

                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IUserService>();
                var user = await service.GetUserByIdAsync(userId);
                if (user is null) return;

                update(user, input);
                var result = await service.UpdateUserAsync(user);

                if (result.Success)
                {
                    SharedUIMethods.PrintMessagePause(result.Message);
                    return;
                }

                foreach (var error in result.Errors)
                    Console.WriteLine($"  {error.Value}");
            }
        }

        internal async Task UpdatePassword(int userId)
        {
            while (true)
            {
                Console.Write("New password (0 to cancel): ");
                var password = SharedUIMethods.ReadPassword();
                if (password == "0") return;

                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IUserService>();
                var user = await service.GetUserByIdAsync(userId);

                var result = await service.ChangePasswordAsync(user!, password);

                if (result.Success)
                {
                    SharedUIMethods.PrintMessagePause(result.Message);
                    return;
                }

                foreach (var error in result.Errors)
                    Console.WriteLine($"  {error.Value}");
            }
        }

        internal async Task UpdateDepartment(int userId)
        {
            var departments = string.Join(", ", Enum.GetValues<Department>());
            Console.WriteLine($"Departments: {departments}");

            while (true)
            {
                Console.Write("New department: ");
                if (!Enum.TryParse<Department>(Console.ReadLine(), true, out var department))
                {
                    Console.WriteLine("Invalid department.");
                    continue;
                }

                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IUserService>();
                var user = await service.GetUserByIdAsync(userId) as Employee;

                user!.Department = department;
                var result = await service.UpdateUserAsync(user);

                if (result.Success)
                {
                    SharedUIMethods.PrintMessagePause(result.Message);
                    return;
                }

                foreach (var error in result.Errors)
                    Console.WriteLine($"  {error.Value}");
            }
        }

        internal async Task<bool> DeleteUser(int userId)
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IUserService>();
            var user = await service.GetUserByIdAsync(userId);

            Console.WriteLine("Are you sure you want to delete the account?");
            Console.WriteLine("Press Y for Yes. Press any other key for No.\n");
            Console.Write("Confirm: ");
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Y)
            {
                var result = await service.DeleteUserAsync(user!.Id);

                if (result.Success)
                    return true;
                else
                    SharedUIMethods.PrintMessagePause(result.Message);
            }
            return false;
        }
    }
}
