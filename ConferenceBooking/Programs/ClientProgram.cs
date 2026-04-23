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
            var selection = await bookingViewer.Show(); //running booking schedule
            Console.Clear();

            if (selection is null) //if no booking was made (returning null) return back to main menu
                return;

            var (room, start, end) = selection.Value;
            var selectedAddOns = await SelectAddOnsAddToBooking(); //displaying choice of adding add ons to booking

            while (true)
            {
                using var scope = scopeFactory.CreateScope();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                var result = await bookingService.CreateBookingAsync(user, room.Id, start, end, selectedAddOns);

                if (result.Success)
                {
                    SharedUIMethods.PrintMessagePause(result.Message);
                    return;
                }

                foreach (var error in result.Errors)
                    Console.WriteLine($"  {error.Value}");

                while (true)
                {
                    Console.Write("New start hour: ");
                    if (!int.TryParse(Console.ReadLine(), out var startHour))
                    {
                        Console.WriteLine("Invalid input.");
                        continue;
                    }
                    if (startHour == 0) return;

                    Console.Write("End hour: ");
                    if (!int.TryParse(Console.ReadLine(), out var endHour))
                    {
                        Console.WriteLine("Invalid input.");
                        continue;
                    }

                    start = start.Date.AddHours(startHour);
                    end = start.Date.AddHours(endHour);
                    break;
                }
            }
        }

        private async Task<List<int>> SelectAddOnsAddToBooking()
        {
            using var scope = scopeFactory.CreateScope();
            var addOnService = scope.ServiceProvider.GetRequiredService<IAddOnService>();
            var addOns = await addOnService.GetAllAddOnsAsync();

            List<int> selectedAddOns = [];

            if (addOns.Count == 0)
            {
                SharedUIMethods.PrintMessagePause("No add ons available.");
                return selectedAddOns;
            }

            var options = addOns.Select(a => $"{a.Name.PadRight(15)} {a.PricePerPerson, 10:C}/person");
            while (true)
            {
                var key = Menu.Show("Select add on", "Back", options.ToArray());
                if (selectedAddOns.Count == addOns.Count) //if all addon options have been added return
                    return selectedAddOns;
                if (key.Key == ConsoleKey.D0) // if key 0 is pressed then break
                    return selectedAddOns;

                var index = key.Key - ConsoleKey.D1; //translates a key to an index/int. If ConsoleKey.D3 is pressed, then subtracting ConsolKey.D1 means index 2(id in list)

                if (index >= 0 && index < addOns.Count)
                {
                    var id = addOns[index].Id;
                    if (selectedAddOns.Contains(id))
                    {
                        SharedUIMethods.PrintMessageSleep("Already added.");
                        continue;
                    }
                    selectedAddOns.Add(id);
                    SharedUIMethods.PrintMessagePause($"Added {addOns[index].Name}");
                }
            }
        }

        private async Task ManageOwnClientAccount(User user)
        {
            while (true)
            {
                Console.Clear();
                var key = Menu.Show("Managing profile", "Back", "Past bookings", "Upcoming bookings", "Manage bookings", "Update my details", "Delete account");

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        await GetBookings(user, b => b.StartTime < DateTime.Now, "Past bookings");
                        break;
                    case ConsoleKey.D2:
                        await GetBookings(user, b => b.StartTime > DateTime.Now, "Upcoming bookings");
                        break;
                    case ConsoleKey.D3:
                        await ManageUpcomingBookings(user.Id);
                        break;
                    case ConsoleKey.D4:
                        await _shared.UpdateUserDetails(user.Id);
                        break;
                    case ConsoleKey.D5:
                        var deleted = await _shared.DeleteUser(user.Id);
                        if (deleted)
                        {
                            SharedUIMethods.PrintMessagePause("Your account has been deleted.");
                            await new LogInProgram(scopeFactory).Run();
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
        private async Task<IEnumerable<Booking>> GetBookings(User user, Func<Booking, bool> condition, string title)
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBookingService>();
            var bookings = await service.GetAllBookingsForUserWithDetailsAsync(user.Id);
            var filtered = bookings.Where(condition);

            Console.WriteLine($"\n{title}");
            SharedUIMethods.ListBookings(filtered);

            return filtered;
        }

        private async Task ManageUpcomingBookings(int userId)
        {
            while (true)
            {
                using var scope = scopeFactory.CreateScope();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                var bookings = await bookingService.GetAllBookingsForUserWithDetailsAsync(userId);
                var upcoming = bookings.Where(b => b.StartTime > DateTime.Now).ToList();

                if (upcoming.Count == 0)
                {
                    SharedUIMethods.PrintMessagePause("No upcoming bookings.");
                    return;
                }

                Console.Clear();
                SharedUIMethods.ListBookingsCompact(upcoming, "Upcoming bookings");

                Console.Write("\nEnter booking ID to manage: ");
                string input = Console.ReadLine();
                if (input == "0" || input is null)
                    return;
                if (!int.TryParse(input, out var bookingId))
                {
                    SharedUIMethods.PrintMessagePause("Invalid input.");
                    continue;
                }

                var booking = upcoming.FirstOrDefault(b => b.Id == bookingId);
                if (booking is null)
                {
                    SharedUIMethods.PrintMessageSleep("Booking not found.");
                    continue;
                }

                var deleted = await ManageSingleBooking(booking.Id);
                if (deleted) return;
            }
        }

        private async Task<bool> ManageSingleBooking(int bookingId)
        {
            while (true)
            {
                using var scope = scopeFactory.CreateScope();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                var booking = await bookingService.GetBookingByIdWithDetailsAsync(bookingId);

                if (booking is null)
                {
                    SharedUIMethods.PrintMessagePause("Booking not found.");
                    return false;
                }

                var key = Menu.Show($"Booking {booking.Id}", "Back",
                    $"Change time {booking.StartTime:HH}-{booking.EndTime:HH}",
                    $"Add add ons",
                    $"Remove add on",
                    $"Cancel booking");

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        await UpdateBookingTime(bookingId);
                        break;
                    case ConsoleKey.D2:
                        await AddAddonToBooking(bookingId);
                        break;
                    case ConsoleKey.D3:
                        await RemoveAddonFromBooking(bookingId);
                        break;
                    case ConsoleKey.D4:
                        var result = await bookingService.DeleteBookingAsync(bookingId);
                        SharedUIMethods.PrintResultMessage(result);
                        if (result.Success) return true;
                        break;
                    case ConsoleKey.D0:
                        return false;
                }
            }
        }

        private async Task UpdateBookingTime(int bookingId)
        {
            while (true)
            {
                Console.Write("New start hour: ");
                if (!int.TryParse(Console.ReadLine(), out var startHour))
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }
                if (startHour == 0) return;

                Console.Write("New end hour: ");
                if (!int.TryParse(Console.ReadLine(), out var endHour))
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }
                if (endHour == 0) return;

                using var scope = scopeFactory.CreateScope();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                var booking = await bookingService.GetBookingByIdAsync(bookingId);

                booking!.StartTime = booking.StartTime.Date.AddHours(startHour);
                booking.EndTime = booking.EndTime.Date.AddHours(endHour);

                var result = await bookingService.UpdateBookingAsync(booking);

                if (result.Success)
                {
                    SharedUIMethods.PrintResultMessage(result);
                    return;
                }

                foreach (var error in result.Errors)
                    Console.WriteLine($"  {error.Value}");
            }
        }

        private async Task AddAddonToBooking(int bookingId)
        {
            using var scope = scopeFactory.CreateScope();
            var addonService = scope.ServiceProvider.GetRequiredService<IAddOnService>();
            var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
            var booking = await bookingService.GetBookingByIdWithDetailsAsync(bookingId);

            if (booking is null)
            {
                SharedUIMethods.PrintMessagePause("Booking not found.");
                return;
            }

            var allAddons = await addonService.GetAllAddOnsAsync();
            var available = allAddons
                .Where(a => !booking.BookingAddOns.Any(ba => ba.AddOnId == a.Id)) //gets all add ons not already in booking
                .ToList();

            if (available.Count == 0)
            {
                SharedUIMethods.PrintMessagePause("No more add ons available.");
                return;
            }

            Console.WriteLine("\nAvailable add ons:");
            foreach (var addon in available)
                Console.WriteLine($"  [{addon.Id}] {addon.Name} - {addon.PricePerPerson:C0}/person");

            while (true)
            {
                Console.Write("\nEnter add on ID: ");
                if (!int.TryParse(Console.ReadLine(), out var addonId))
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }
                if (addonId == 0) return;

                var result = await bookingService.AddAddonToBookingAsync(bookingId, addonId);
                SharedUIMethods.PrintResultMessage(result);
                return;
            }
        }

        private async Task RemoveAddonFromBooking(int bookingId)
        {
            using var scope = scopeFactory.CreateScope();
            var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
            var booking = await bookingService.GetBookingByIdWithDetailsAsync(bookingId);

            if (booking is null)
            {
                SharedUIMethods.PrintMessagePause("Booking not found.");
                return;
            }

            if (booking.BookingAddOns.Count == 0)
            {
                SharedUIMethods.PrintMessagePause("No add ons to remove.");
                return;
            }

            Console.WriteLine("\nCurrent add ons:");
            var addonList = booking.BookingAddOns.ToList();
            for (int i = 0; i < addonList.Count; i++)
                Console.WriteLine($"[{i + 1}] {addonList[i].AddOn.Name} - {addonList[i].TotalPrice:C0}");

            while (true)
            {
                Console.Write("\nSelect add on to remove: ");
                if (!int.TryParse(Console.ReadLine(), out var choice))
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }
                if (choice == 0) return;

                var index = choice - 1;
                if (index < 0 || index >= addonList.Count)
                {
                    Console.WriteLine("Invalid choice.");
                    continue;
                }

                var result = await bookingService.RemoveAddonFromBookingAsync(bookingId, addonList[index].Id);
                SharedUIMethods.PrintMessagePause(result.Message);
                return;
            }
        }
    }
}
