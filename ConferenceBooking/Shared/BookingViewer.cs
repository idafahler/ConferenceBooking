using ConferenceBooking.Application.ServiceInterfaces;
using ConferenceBooking.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Presentation.Shared
{
    internal class BookingViewer(IServiceScopeFactory scopeFactory)
    {
        private int _weekOffset = 0;

        public async Task<(ConferenceRoom room, DateTime start, DateTime end)?> Show()
        {
            while (true)
            {
                Console.Clear();
                var weekStart = GetMondayOfWeek(_weekOffset);

                using var scope = scopeFactory.CreateScope();
                var roomService = scope.ServiceProvider.GetRequiredService<IConferenceRoomService>();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

                var rooms = await roomService.GetAllRoomsAsync();
                var bookings = await GetBookingsForWeek(bookingService, weekStart);

                PrintHeader(weekStart);
                PrintGrid(weekStart, rooms, bookings);
                PrintNavigationAndRoomInfo(rooms);

                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.RightArrow when _weekOffset < 2: _weekOffset++; //changes week only when condition is true
                        break;
                    case ConsoleKey.LeftArrow when _weekOffset > -1: _weekOffset--;
                        break;
                    case ConsoleKey.Escape:
                        return null;
                    default:
                        if (char.IsDigit(key.KeyChar))
                        {
                            var id = key.KeyChar - '0';
                            var room = rooms.FirstOrDefault(r => r.Id == id);
                            if (room is not null)
                            {
                                var result = await SelectDayAndTime(weekStart, room, bookings); //returns tuple
                                if (result is not null)
                                {
                                    return result;
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void PrintHeader(DateTime weekStart)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"== Room Schedule - Week {GetWeekNumber(weekStart)} ==\n");
            Console.ResetColor();

            Console.Write($"{"",10}");
            for (int i = 0; i < 5; i++)
            {
                var day = weekStart.AddDays(i);
                var dayName = day.ToString("ddd", System.Globalization.CultureInfo.InvariantCulture);
                Console.Write($"{dayName + " " + day.ToString("dd/MM"),14}");
            }
            Console.WriteLine();
            Console.WriteLine(new string('-', 80));
        }

        private void PrintGrid(DateTime weekStart, List<ConferenceRoom> rooms, List<Booking> bookings)
        {
            for (int r = 0; r < rooms.Count; r++)
            {
                var room = rooms[r];
                Console.Write($"{room.Number,-10}");

                for (int day = 0; day < 5; day++)
                {
                    var date = weekStart.AddDays(day);
                    var status = GetDayStatus(date, room.Id, bookings);

                    Console.ForegroundColor = status switch
                    {
                        DayStatus.Passed => ConsoleColor.DarkGray,
                        DayStatus.Available => ConsoleColor.Green,
                        DayStatus.PartiallyBooked => ConsoleColor.DarkYellow,
                        DayStatus.FullyBooked => ConsoleColor.Red,
                        _ => ConsoleColor.White
                    };

                    var label = status switch
                    {
                        DayStatus.Passed => "Passed",
                        DayStatus.Available => "Available",
                        DayStatus.PartiallyBooked => "Partial",
                        DayStatus.FullyBooked => "Booked",
                        _ => ""
                    };

                    Console.Write($"{label,14}"); //writes out status of room for each day.
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        private DayStatus GetDayStatus(DateTime date, int roomId, List<Booking> bookings)
        {
            if (date < DateTime.Today)
                return DayStatus.Passed;

            var dayBookings = bookings //gets bookings for room on day
                .Where(b => b.ConferenceRoomId == roomId && b.StartTime.Date == date.Date)
                .ToList();

            int availableHours = 0;
            int bookedHours = 0;

            for (int hour = 8; hour < 17; hour++) //iterates over each hour
            {
                var slotTime = date.AddHours(hour);

                if (slotTime <= DateTime.Now) //hours already past today does not count
                    continue;

                if (dayBookings.Any(b => slotTime >= b.StartTime && slotTime < b.EndTime)) //checks if room is booked this hour
                    bookedHours++;
                else
                    availableHours++;
            }

            if (availableHours == 0 && bookedHours == 0) //nothing available and nothing booked. day has passed
                return DayStatus.Passed;

            if (availableHours == 0) //no available hours, everything is booked
                return DayStatus.FullyBooked;

            if (bookedHours > 0) // some booked hours but not all, partially booked day.
                return DayStatus.PartiallyBooked;

            return DayStatus.Available; //last case, everything is available
        }

        private void PrintNavigationAndRoomInfo(List<ConferenceRoom> rooms)
        {
            Console.WriteLine();
            Console.WriteLine(new string('-', 80));

            Console.WriteLine($"\nId {"Room",8} {"Capacity",14} {"Price/ hour",16}");
            Console.WriteLine($"{new string('-', 45)}");

            foreach (var room in rooms)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"[{room.Id}] ");
                Console.ResetColor();
                Console.WriteLine($"{room.Number,7} {room.Capacity,14} {room.PricePerHour,16:C0}");
            }

            Console.WriteLine();
            Console.WriteLine($"[1-{rooms.Count}] Select room");
            Console.WriteLine("[</>] Previous/Next week");
            Console.WriteLine("[Esc] Cancel");
        }

        private async Task<(ConferenceRoom room, DateTime start, DateTime end)?> SelectDayAndTime(
            DateTime weekStart, ConferenceRoom room, List<Booking> bookings)
        {
            Console.Write("\nSelect day: [1]Mon [2]Tue [3]Wed [4]Thu [5]Fri: ");
            if (!int.TryParse(Console.ReadLine(), out int dayChoice) || dayChoice < 1 || dayChoice > 5)
            {
                SharedUIMethods.PrintMessageSleep("Invalid day.");
                return null;
            }

            var selectedDate = weekStart.AddDays(dayChoice - 1); //taking choice - 1 to translate to weekday. Monday == weekstart

            if (selectedDate < DateTime.Today)
            {
                SharedUIMethods.PrintMessageSleep("Invalid date.");
                return null;
            }
            if(GetDayStatus(selectedDate, room.Id, bookings) == DayStatus.FullyBooked)
            {
                SharedUIMethods.PrintMessageSleep("Room is fully booked this day.");
                return null;
            }

            Console.Clear();
            Console.WriteLine($"\nRoom: {room.Number} - {selectedDate:ddd dd/MM}:");
            var dayBookings = bookings
                .Where(b => b.ConferenceRoomId == room.Id && b.StartTime.Date == selectedDate.Date)
                .ToList(); //gets bookings for day
            
            for (int hour = 8; hour < 17; hour++)
            {
                var slotTime = selectedDate.AddHours(hour);

                if (slotTime <= DateTime.Now) //unavailable times of day to book, already passed in time
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"  {hour:00}:00 - Passed");
                }
                else if (dayBookings.Any(b => slotTime >= b.StartTime && slotTime < b.EndTime)) //booked hours
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  {hour:00}:00 - Booked");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  {hour:00}:00 - Available");
                }
                Console.ResetColor();
            }

            //select duration for booking
            Console.Write("\nStart hour: "); 
            if (!int.TryParse(Console.ReadLine(), out int startHour))
            {
                SharedUIMethods.PrintMessageSleep("Invalid input.");
                return null;
            }
            if (startHour == 0) return null;

            Console.Write("End hour: ");
            if (!int.TryParse(Console.ReadLine(), out int endHour))
            {
                SharedUIMethods.PrintMessageSleep("Invalid input.");
                return null;
            }
            if(endHour == 0) return null;

            var start = selectedDate.AddHours(startHour);
            var end = selectedDate.AddHours(endHour);

            return (room, start, end); //returns room and duration for booking
        }

        private async Task<List<Booking>> GetBookingsForWeek(IBookingService service, DateTime weekStart) //gets all bookings choosen week
        {
            var weekEnd = weekStart.AddDays(5);
            var bookings = await service.FindBookingsAsync(b =>
                b.StartTime >= weekStart &&
                b.StartTime < weekEnd);
            return bookings.ToList();
        }

        private DateTime GetMondayOfWeek(int offset) //gets monday of week
        {
            var today = DateTime.Today;
            var diff = (int)today.DayOfWeek - (int)DayOfWeek.Monday;
            if (diff < 0) diff += 7;
            return today.AddDays(-diff + (offset * 7));
        }

        private int GetWeekNumber(DateTime date) //gets week number
        {
            var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            return cal.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private enum DayStatus
        {
            Passed,
            Available,
            PartiallyBooked,
            FullyBooked
        }
    }
}
