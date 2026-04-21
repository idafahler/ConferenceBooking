using ConferenceBooking.Application;
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
    internal class ClientProgram(IServiceScopeFactory scopeFactory)
    {
        private readonly SharedOperations _shared = new(scopeFactory);
        internal async Task Run(User user)
        {
            while (true)
            {
                Console.Clear();
                var key = Menu.Show($"Logged in", "Log out", "Book a conference room", "My profile");

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        await Booking(user);
                        break;
                    case ConsoleKey.D2:
                        await ManageOwnClientAccount(user);
                        break;
                    case ConsoleKey.D0:
                        return;
                    default:
                        SharedUIMethods.PrintMessageSleep("Invalid choice.");
                        break;
                }
            }
        }

        private async Task Booking(User user)
        {
            var bookingViewer = new BookingViewer(scopeFactory);
            var selection = await bookingViewer.Show();

            if (selection is null)
                return;

            var (room, start, end) = selection.Value;

            var selectedAddOns = await SelectAddOns();

            ServiceResult result;
            do
            {
                using var scope = scopeFactory.CreateScope();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                result = await bookingService.CreateBookingAsync(user, room.Id, start, end, selectedAddOns);

                if (!result.Success)
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"{error.Value}");

                        if (error.Key is "Time" or "Conflict")
                        {
                            Console.Write("Error. Enter new start hour (8-16): ");
                            var startHour = int.Parse(Console.ReadLine()!);
                            Console.Write("Number of hours: ");
                            var hours = int.Parse(Console.ReadLine()!);
                            start = start.Date.AddHours(startHour);
                            end = start.Date.AddHours(startHour + hours);
                        }
                    }
                }

            } while (!result.Success);

            SharedUIMethods.PrintResultMessage(result);
        }

        private async Task<List<int>> SelectAddOns()
        {
            using var scope = scopeFactory.CreateScope();
            var addOnService = scope.ServiceProvider.GetRequiredService<IAddOnService>();
            var addOns = await addOnService.GetAllAddOnsAsync();

            List<int> selectedAddOns = [];

            if (addOns.Count == 0)
            {
                SharedUIMethods.PrintMessagePause("No add-ons available.");
                return selectedAddOns;
            }
            SharedUIMethods.ListAddOns(addOns);

            Console.WriteLine("Would you like to add anything?");
            Console.WriteLine("Press add on ID to add, [0] to finish.\n");
            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.KeyChar == '0') break;

                if (!char.IsDigit(key.KeyChar))
                    continue;

                int id = key.KeyChar - '0';

                var addOn = await addOnService.GetAddOnByIdAsync(id);

                if (addOn is null)
                {
                    SharedUIMethods.PrintMessageSleep("Add on not found.");
                    continue;
                }
                selectedAddOns.Add(id);
                Console.WriteLine($"Added: {addOn.Name}");
            }
            return selectedAddOns;
        }

        private async Task ManageOwnClientAccount(User user)
        {
            while (true)
            {
                Console.Clear();
                var key = Menu.Show("Managing profile", "Back", "Past bookings", "Upcoming bookings", "Update my details", "Delete account");

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        await GetBookings(user, b => b.StartTime < DateTime.Now, "Past bookings");
                        break;
                    case ConsoleKey.D2:
                        await GetBookings(user, b => b.StartTime > DateTime.Now, "Upcoming bookings");
                        break;
                    case ConsoleKey.D3:
                        await _shared.UpdateUserDetails(user.Id);
                        break;
                    case ConsoleKey.D4:
                        var deleted = await _shared.DeleteUser(user.Id);
                        if (deleted)
                        {
                            SharedUIMethods.PrintMessagePause("Your account has been deleted. Exiting program.");
                            Environment.Exit(0);
                        }
                        break;
                    case ConsoleKey.D0:
                        return;
                    default:
                        SharedUIMethods.PrintMessageSleep("Invalid choice.");
                        break;
                }
            }
        }

        private async Task GetBookings(User user, Func<Booking, bool> condition, string title)
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBookingService>();
            var bookings = await service.GetAllBookingsForUserWithDetailsAsync(user.Id);
            var filtered = bookings.Where(condition);

            Console.WriteLine($"\n{title}");
            SharedUIMethods.ListBookings(filtered);
        }
    }
}
