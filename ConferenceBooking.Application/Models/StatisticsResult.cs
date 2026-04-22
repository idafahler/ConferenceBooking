using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Application.Models
{
    public record StatisticsResult(
    string MostBookedRoom,
    int MostBookedRoomCount,
    string MostPopularAddon,
    int MostPopularAddonCount,
    List<RoomRevenueInfo> RevenuePerRoom,
    List<RoomOccupancyInfo> OccupancyPerRoom);

    public record RoomRevenueInfo(string RoomNumber, decimal Revenue);

    public record RoomOccupancyInfo(string RoomNumber, double OccupancyPercent);
}
