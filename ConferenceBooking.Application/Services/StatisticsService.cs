using ConferenceBooking.Application.Models;
using ConferenceBooking.Application.RepositoryInterfaces;
using ConferenceBooking.Application.ServiceInterfaces;
using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Application.Services
{
    public class StatisticsService(IBookingRepository bookingRepo, IBookingAddOnRepository bookingAddonRepo, IConferenceRoomRepository roomRepo) : IStatisticsService
    {
        public async Task<StatisticsResult> GetStatisticsAsync()
        {
            var bookings = await bookingRepo.GetAllAsync();
            var rooms = await roomRepo.GetAllAsync();
            var allBookingAddons = await bookingAddonRepo.GetAllWithAddonAsync();

            var mostBookedGroup = bookings 
                .GroupBy(b => b.ConferenceRoomId) //grouping bookings by room
                .OrderByDescending(g => g.Count()) //counting amounts of bookings in group and sorting
                .FirstOrDefault();
            var mostBookedRoom = rooms.FirstOrDefault(r => r.Id == mostBookedGroup?.Key); //gets room

            var mostPopular = allBookingAddons
                .GroupBy(ba => ba.AddOnId)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            var revenuePerRoom = bookings
                .GroupBy(b => b.ConferenceRoomId) //group all bookings per rum
                .Select(g => new RoomRevenueInfo( //create new room revenue info record per group
                    rooms.First(r => r.Id == g.Key).Number, //gets name of room
                    g.Sum(b => b.TotalPrice))) //sum of all bookings in room
                .OrderByDescending(r => r.Revenue) //most pricy first
                .ToList();

            var thisWeekStart = GetMondayOfCurrentWeek();
            var thisWeekEnd = thisWeekStart.AddDays(5);

            var occupancyPerRoomThisWeek = rooms.Select(room =>
            {
                var roomBookings = bookings //gets bookings for this week for this room
                    .Where(b => b.ConferenceRoomId == room.Id &&
                                b.StartTime >= thisWeekStart &&
                                b.StartTime < thisWeekEnd)
                    .ToList();

                var totalBookedHours = roomBookings.Sum(b => (b.EndTime - b.StartTime).TotalHours); //sums all booked hours
                var percent = Math.Round((totalBookedHours / 45.0) * 100, 1); //gets procent of occupancy

                return new RoomOccupancyInfo(room.Number, percent);
            })
            .OrderByDescending(o => o.OccupancyPercent)
            .ToList();
            //building together result into record and returning info.
            return new StatisticsResult(
                mostBookedRoom?.Number ?? "-",
                mostBookedGroup?.Count() ?? 0,
                mostPopular?.First().AddOn?.Name ?? "-",
                mostPopular?.Count() ?? 0,
                revenuePerRoom,
                occupancyPerRoomThisWeek);
        }

        private static DateTime GetMondayOfCurrentWeek()
        {
            var today = DateTime.Today;
            var diff = (int)today.DayOfWeek - (int)DayOfWeek.Monday; //gets diff between today and monday. how many days ago was monday?
            if (diff < 0) diff += 7; //if today is sunday. sunday == 0. sunday(0) - monday(1) = -1. -1+7=6
            return today.AddDays(-diff); //returns monday of this week.
        }
    }
}
