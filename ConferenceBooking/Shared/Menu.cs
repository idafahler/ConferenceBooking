using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Presentation.Helper
{
    internal static class Menu
    {
        internal static ConsoleKeyInfo Show(string title, string navigation, params string[] options)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"== {title} ==");
            Console.ResetColor();

            for (int i = 0; i< options.Length; i++)
            {
                Console.WriteLine($"[{i +1}] {options[i]}");
            }
            Console.WriteLine($"[0] {navigation}\n");
            Console.Write("Choose: ");
            return Console.ReadKey(true)!;
        }
    }
}
