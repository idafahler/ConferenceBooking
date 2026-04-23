using ConferenceBooking.Application;
using ConferenceBooking.Application.Models;
using ConferenceBooking.Application.ServiceInterfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Enums;
using ConferenceBooking.Presentation.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Presentation.Programs
{
    internal class LogInProgram(IServiceScopeFactory scopeFactory)
    {
        private readonly SharedOperations _shared = new(scopeFactory);
        internal async Task Run()
        {
            while (true)
            {
                var key = Menu.Show("Conference booking", "Quit", "Login", "Create user");

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        var user = await LogIn();

                        if (user is Admin)
                            await new AdminProgram(scopeFactory).Run(user);

                        else if (user is Employee or ExternalUser)
                            await new ClientProgram(scopeFactory).Run(user);

                        break;

                    case ConsoleKey.D2:
                        await _shared.CreateNewUser(UserType.ExternalUser);
                        break;

                    case ConsoleKey.D0:
                        SharedUIMethods.PrintMessagePause("Exiting program.");
                        return;

                    default:
                        SharedUIMethods.PrintMessageSleep("Invalid choice.");
                        break;
                }
            }
        }

        private async Task<User?> LogIn()
        {
            Console.Write("Username: ");
            var username = Console.ReadLine();
            if (username == "0") return null;
            Console.Write("Password: ");
            var password = SharedUIMethods.ReadPassword();
            if (password == "0") return null;

            using var scope = scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            ServiceResult<User> result = await userService.AuthenticateUserAsync(username!, password);
            SharedUIMethods.PrintResultMessage(result);

            return result.Data;
        }
    }
}
