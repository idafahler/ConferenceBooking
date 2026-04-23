using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Application.Models
{
    //record for statistics. record instead of a class because the data will only be shown not changed. properties in records are init, after initialization they cannot be changes.
    public record StatisticsResult(
    string MostBookedRoom,
    int MostBookedRoomCount,
    string MostPopularAddon,
    int MostPopularAddonCount,
    List<RoomRevenueInfo> RevenuePerRoom,
    List<RoomOccupancyInfo> OccupancyPerRoom);

    public record RoomRevenueInfo(string RoomNumber, decimal Revenue); //records/classes should be in separate files, but these were related to the same operation and small.

    public record RoomOccupancyInfo(string RoomNumber, double OccupancyPercent);
}
