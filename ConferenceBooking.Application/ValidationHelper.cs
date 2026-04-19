using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Application
{
    public static class ValidationHelper
    {
        internal static string? ValidateName<T>(string? name, List<T> existing, Func<T, string> selector)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Name is missing";

            if (name.Length < 2)
                return "Name must be at least 2 characters long.";

            if (name.Length > 64)
                return "Name must be under 64 characters long.";

            if (existing.Any(x => selector(x).ToLower() == name.ToLower()))
                return "Name already exists.";

            return null;
        }

        internal static string? ValidatePrice(decimal price)
        {
            if (price < 0)
                return "Price can't be negative.";

            if (price == 0)
                return "Price can't be zero.";

            return null;
        }

        internal static string? ValidateTimeRange(DateTime? startTime, DateTime? endTime)
        {
            if (startTime >= endTime)
                return "Start time must be before end time.";
            if (startTime < DateTime.Now)
                return "Start time cannot be in the past";

            return null;
        }

        internal static string? ValidateCapacity(int capacity)
        {
            if (capacity < 0)
                return "Capacity cannot be negative";

            if (capacity == 0)
                return "Capacity cannot be zero.";

            return null;
        }
    }
}
