using ConferenceBooking.Application;
using ConferenceBooking.Application.ServiceInterfaces;
using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Presentation.Shared
{
    internal class SharedUIMethods
    {
        internal static void PrintResultMessage(ServiceResult result)
        {
            Console.WriteLine($"\n{result.Message}");
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey(true);
        }

        internal static void PrintMessagePause(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey(true);
        }

        internal static void PrintMessageSleep(string message)
        {
            Console.WriteLine(message);
            Thread.Sleep(1500);
        }

        internal static string ReadPassword()
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

        internal static void ListBookings(IEnumerable<Booking> bookings, bool showUser = false)
        {
            if(!bookings.Any())
            {
                PrintMessagePause("No bookings available.");
                return;
            }

            foreach(var booking in bookings)
            {
                Console.WriteLine($"""
                    Booking Id: {booking.Id}
                        Room: {booking.ConferenceRoom.Number}
                        Date: {booking.StartTime:yyyy-MM-dd}
                        Time: {booking.StartTime:HH}-{booking.EndTime:HH}
                        Total: {booking.TotalPrice:C}
                    """);
                if(showUser)
                    Console.WriteLine($"    User: {booking.User.FullName}");
                if (booking.BookingAddOns.Count != 0)
                {
                    var addonList = booking.BookingAddOns
                        .Select(ba => $"{ba.AddOn.Name}")
                        .ToList();

                    Console.WriteLine($"    Add ons: {string.Join(", ", addonList)}");
                }
                Console.WriteLine();
            }
            PrintMessagePause("");
            return;
        }

        internal static void ListAddOns(List<AddOn> addOns)
        {
            if(addOns.Count == 0)
            {
                Console.Clear();
                PrintMessagePause("No add ons available.");
                return;
            }

            Console.WriteLine("All add ons:");
            foreach(var addOn in addOns)
            {
                Console.WriteLine($"[{addOn.Id}] {addOn.Name} - {addOn.PricePerPerson:C}/person");
            }
            Console.WriteLine();
            return;
        }

        internal static void ListBookingsCompact(List<Booking> bookings, string title)
        {
            Console.WriteLine($"{title}");
            Console.WriteLine($"{"ID",-8} {"Room",-11} {"Date",-14} {"Time",-16} {"Total",7}");
            Console.WriteLine($"{new string('-', 60)}");

            foreach (var booking in bookings)
            {
                var date = booking.StartTime.ToString("yyyy-MM-dd");
                var time = $"{booking.StartTime:HH:00} - {booking.EndTime:HH:00}";

                Console.WriteLine($"{booking.Id,-10}{booking.ConferenceRoom.Number,-10}{date,-14}{time,-16}{booking.TotalPrice,10:C0}");

                if (booking.BookingAddOns.Count != 0)
                {
                    var addonList = booking.BookingAddOns
                        .Select(ba => $"{ba.AddOn.Name} (x{ba.Quantity})")
                        .ToList();

                    Console.WriteLine($"    Add ons: {string.Join(", ", addonList)}");
                }
            }
        }
    }
}
