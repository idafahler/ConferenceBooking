using ConferenceBooking.Application;
using ConferenceBooking.Application.Interfaces;
using ConferenceBooking.Application.Models;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Enums;
using ConferenceBooking.Presentation.Helper;
using ConferenceBooking.Presentation.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Presentation.Programs
{
    internal class LogInProgram(IServiceScopeFactory scopeFactory)
    {
        internal async Task Run()
        {
            while (true)
            {
                var key = Menu.Show("Conference booking", "Quit", "Login", "Create user");

                if (key.Key == ConsoleKey.D1)
                {
                    var user = await LogIn();
                    if (user is null)
                        continue;
                    else if (user is Admin)
                        await new AdminProgram(scopeFactory).Run(user);
                    else if (user is Employee or ExternalUser)
                        await new ClientProgram(scopeFactory).Run(user);
                }
                else if (key.Key == ConsoleKey.D2)
                {
                    await CreateNewUser();
                }
                else if (key.Key == ConsoleKey.D0)
                    return;
                else
                    SharedMethods.PrintMessage("Press any key to quit program.");
            }
        }

        private async Task<User?> LogIn()
        {
            Console.Write("Username: ");
            var username = Console.ReadLine();
            Console.Write("Password: ");
            var password = ReadPassword();

            using var scope = scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            ServiceResult<User> result = await userService.AuthenticateUserAsync(username!, password);
            SharedMethods.PrintResultMessage(result);

            if (!result.Success)
                return null;

            return result.Data;
        }

        private static string ReadPassword()
        {
            var password = new StringBuilder();

            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                    break;

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return password.ToString();
        }

        private async Task CreateNewUser()
        {
            var key = Menu.Show("Select role", "Back", "Employee", "External");

            UserType role;
            switch (key.Key)
            {
                case ConsoleKey.D1:
                    role = UserType.Employee;
                    break;
                case ConsoleKey.D2:
                    role = UserType.ExternalUser;
                    break;
                default:
                    SharedMethods.PrintMessage("Invalid. You will be sent back.");
                    return;
            }

            Console.Write("\nUsername: ");
            var username = Console.ReadLine()!;
            Console.Write("Password: ");
            var password = ReadPassword();
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
                    details.Password = ReadPassword();
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
                        if (fixers.ContainsKey(error.Key))
                            fixers[error.Key]();
                    }
                }
            } while (!result.Success);

            SharedMethods.PrintResultMessage(result);
        }
    }
}
