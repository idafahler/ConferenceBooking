using ConferenceBooking.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Presentation.Shared
{
    internal class SharedMethods
    {
        internal static void PrintResultMessage(ServiceResult result)
        {
            Console.WriteLine($"\n{result.Message}");
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey(true);
        }

        internal static void PrintMessage(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey(true);
        }
    }
}
