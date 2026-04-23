using ConferenceBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Application
{
    public static class ValidationHelper
    {
        public static string? ValidateUniqueName<T>(string? name, string property, List<T> existing, Func<T, string> selector)
        {
            if (string.IsNullOrWhiteSpace(name))
                return $"{property} is required";

            name = name.Trim();

            if (name.Length < 2)
                return $"{property} must be at least 2 characters long.";

            if (name.Length > 64)
                return $"{property} must be under 64 characters long.";

            if (existing.Select(selector).Contains(name, StringComparer.OrdinalIgnoreCase))
                return $"{property} already exists";

            return null;
        }

        public static string? ValidateName(string? name, string property)
        {
            if (string.IsNullOrWhiteSpace(name))
                return $"{property} is required";

            name = name.Trim();

            if (name.Length < 2)
                return $"{property} must be at least 2 characters long.";

            if (name.Length > 64)
                return $"{property} must be under 64 characters long.";

            if (name.All(char.IsDigit))
                return $"{property} cannot be only digits.";

            if (name.Count(char.IsLetter) < 2)
                return $"{property} must contain at least 2 letters";

            if (name.Contains("  "))
                return $"{property} cannot contain consecutive spaces";

            if (name.Count(' ') > 3)
                return $"{property} cannot contain more than 3 spaces";

            return null;
        }

        internal static string? ValidateEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "Email is required.";

            email = email.Trim();

            if (email.Count(c => c == '@') != 1 || !email.Contains('.') ||
                email.IndexOf('@') < 2 || email.IndexOf('@') >= email.Length - 1)
                return "Email is invalid.";

            return null;
        }

        public static string? ValidatePassWord(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "Password is required";

            if (password.Length < 6)
                return "Password must be at least 6 characters long.";

            if (password.Length > 128)
                return "Password is too long.";

            if (!password.Any(char.IsLetter))
                return "Password must contain at least one letter.";

            if (!password.Any(char.IsDigit))
                return "Password must contain at least one number.";

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                return "Password must contain at least one special character.";

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
            if (startTime is null || endTime is null)
                return "Startime and endtime are required.";
            if (startTime >= endTime)
                return "Start time must be before end time.";
            if (startTime < DateTime.Now)
                return "Start time cannot be in the past";
            if (startTime.Value.Hour < 8 || startTime.Value.Hour > 16)
                return "Start time must be between 08:00 and 16:00.";
            if (endTime.Value.Hour < 9 || endTime.Value.Hour > 17)
                return "End time must be between 09:00 and 17:00";

                return null;
        }

        internal static string? ValidateNoBookingConflict(DateTime startTime, DateTime endTime, int roomId,
            List<Booking> existingBookings, int? excludeBookingId = null)
        {
            var conflict = existingBookings
                .Where(b => b.ConferenceRoomId == roomId)
                .Where(b => excludeBookingId == null || b.Id != excludeBookingId)
                .Any(b => b.StartTime < endTime && b.EndTime > startTime);
            if (conflict)
                return "The room is already booked during this time.";

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

        internal static string? ValidateAmountInstances<T>(List<T> existing, string entity)
        {
            if (existing.Count >= 9)
            {
                return $"Cannot create more than 9 {entity}";
            }
            return null;
        }
    }
}
