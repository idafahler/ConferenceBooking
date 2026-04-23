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
    internal class AdminProgram(IServiceScopeFactory scopeFactory)
    {
        private readonly SharedOperations _shared = new(scopeFactory);
        internal async Task Run(User user)
        {
            while (true)
            {
                Console.Clear();
                var key = Menu.Show($"Logged in", "Log out", "My profile", "Add new account", "Managade add ons", "Manage conference rooms", "Managade room features", "See statistics");

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        await ManageOwnAdminAccount(user);
                        break;
                    case ConsoleKey.D2:
                        await AddNewAccount();
                        break;
                    case ConsoleKey.D3:
                        await ManageAddOns();
                        break;
                    case ConsoleKey.D4:
                        await ManageConferenceRooms();
                        break;
                    case ConsoleKey.D5:
                        await ManageFeatures();
                        break;
                    case ConsoleKey.D6:
                        await SeeStatistics();
                        break;
                    case ConsoleKey.D0:
                        return;
                    default:
                        SharedUIMethods.PrintMessageSleep("Invalid choice.");
                        break;
                }
            }
        }

        private async Task ManageOwnAdminAccount(User user)
        {
            while (true)
            {
                Console.Clear();
                var key = Menu.Show("Managing profile", "Back", "Update my details", "Delete account");

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        await _shared.UpdateUserDetails(user.Id);
                        break;
                    case ConsoleKey.D2:
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

        private async Task AddNewAccount()
        {
            Console.Clear();
            var key = Menu.Show("Select role", "Back", "Admin", "Employee");

            UserType role;
            switch (key.Key)
            {
                case ConsoleKey.D1:
                    role = UserType.Admin;
                    break;
                case ConsoleKey.D2:
                    role = UserType.Employee;
                    break;
                default:
                    SharedUIMethods.PrintMessagePause("Invalid. You will be sent back.");
                    return;
            }
            await _shared.CreateNewUser(role);
        }


        internal async Task ManageAddOns()
        {
            while (true)
            {
                Console.Clear();
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IAddOnService>();
                var addOns = await service.GetAllAddOnsAsync();
                if (addOns.Count == 0)
                    return;

                var options = addOns.Select(a => $"{a.Name,-20} {a.PricePerPerson,-10:C}/person");

                if (addOns.Count < 9)
                    options = options.Append("Create new add on");

                var key = Menu.Show("Manage Add Ons", "Back", options.ToArray());

                if (key.Key == ConsoleKey.D0) 
                    return;

                var index = key.Key - ConsoleKey.D1; //ConsoleKey.D1 - ConsoleKey.D1 == 0

                if (index == addOns.Count) //Last option
                    await CreateAddOn();
                else if (index >= 0 && index < addOns.Count)
                    await ManageSingleAddOn(addOns[index].Id);
            }
        }

        private async Task ManageSingleAddOn(int addOnId)
        {
            while (true)
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IAddOnService>();
                var addOn = await service.GetAddOnByIdAsync(addOnId);

                if (addOn is null)
                {
                    SharedUIMethods.PrintMessagePause("Add on not found.");
                    return;
                }

                var key = Menu.Show($"Managing: {addOn.Name}", "Back", $"Name: {addOn.Name}",
                    $"Price: {addOn.PricePerPerson:C}/person", "Delete");

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        await UpdateAddOnName(addOnId);
                        break;
                    case ConsoleKey.D2:
                        await UpdateAddOnPrice(addOnId);
                        break;
                    case ConsoleKey.D3:
                        var result = await service.DeleteAddOnAsync(addOnId);
                        SharedUIMethods.PrintResultMessage(result);
                        if (result.Success) return;
                        break;
                    case ConsoleKey.D0:
                        return;
                }
            }
        }

        private async Task UpdateAddOnName(int addOnId)
        {
            ServiceResult result;
            do
            {
                Console.Write("Enter new name: ");
                var input = Console.ReadLine()!;
                if (input == "0") return;

                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IAddOnService>();
                var addOn = await service.GetAddOnByIdAsync(addOnId);
                if(addOn is null)
                {
                    SharedUIMethods.PrintMessagePause("Could not find add on.");
                    return;
                }

                addOn!.Name = input;
                result = await service.UpdateAddOnAsync(addOn);

                if (!result.Success)
                {
                    foreach (var error in result.Errors)
                        Console.WriteLine($"  {error.Value}");
                }
            } while (!result.Success);

            SharedUIMethods.PrintResultMessage(result);
        }

        private async Task UpdateAddOnPrice(int addOnId)
        {
            while (true)
            {
                Console.Write("Enter new price: ");
                if (!decimal.TryParse(Console.ReadLine(), out var price) || price >= 99999)
                {
                    Console.WriteLine("Invalid price.");
                    continue;
                }
                if (price == 0) return;

                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IAddOnService>();
                var addOn = await service.GetAddOnByIdAsync(addOnId);
                if (addOn is null)
                {
                    SharedUIMethods.PrintMessagePause("Could not find add on.");
                    return;
                }

                addOn!.PricePerPerson = price;
                var result = await service.UpdateAddOnAsync(addOn);

                if (result.Success)
                {
                    SharedUIMethods.PrintResultMessage(result);
                    return;
                }

                foreach (var error in result.Errors)
                    Console.WriteLine($"  {error.Value}");
            }
        }

        private async Task CreateAddOn()
        {
            Console.Write("Name: ");
            var name = Console.ReadLine()!;

            decimal price;
            while (true)
            {
                Console.Write("Price per person: ");
                if (decimal.TryParse(Console.ReadLine(), out price) && price <= 99999)
                    break;
                Console.WriteLine("Invalid price.");
            }

            var addOn = new AddOn { Name = name, PricePerPerson = price };

            ServiceResult result;
            do
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IAddOnService>();
                result = await service.CreateAddOnAsync(addOn);

                if (!result.Success)
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"  {error.Value}");

                        if (error.Key == "Name")
                        {
                            Console.Write("New name: ");
                            addOn.Name = Console.ReadLine()!;
                        }
                        else if (error.Key == "Price per person")
                        {
                            while (true)
                            {
                                Console.Write("New price: ");
                                if (decimal.TryParse(Console.ReadLine(), out var p))
                                {
                                    addOn.PricePerPerson = p;
                                    break;
                                }
                                Console.WriteLine("Invalid price.");
                            }
                        }
                    }
                }
            } while (!result.Success);

            SharedUIMethods.PrintResultMessage(result);
        }

        private async Task ManageConferenceRooms()
        {
            while (true)
            {
                Console.Clear();
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IConferenceRoomService>();
                var rooms = await service.GetAllRoomsAsync();
                if (rooms.Count == 0)
                    return;

                var options = rooms.Select(r => $"{r.Number,-10} Capacity: {r.Capacity,-5} {r.PricePerHour,10:C}/hr");

                if (rooms.Count < 9)
                    options = options.Append("Create new room");

                var key = Menu.Show("Manage Conference Rooms", "Back", options.ToArray());

                if (key.Key == ConsoleKey.D0)
                    return;

                var index = key.Key - ConsoleKey.D1;

                if (index == rooms.Count)
                    await CreateRoom();
                else if (index >= 0 && index < rooms.Count)
                    await ManageSingleRoom(rooms[index].Id);
            }
        }

        private async Task ManageSingleRoom(int roomId)
        {
            while (true)
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IConferenceRoomService>();
                var room = await service.GetRoomByIdAsync(roomId);

                if (room is null)
                {
                    SharedUIMethods.PrintMessagePause("Room not found.");
                    return;
                }

                var key = Menu.Show($"Managing: {room.Number}", "Back",
                    $"Number: {room.Number}",
                    $"Capacity: {room.Capacity}",
                    $"Price: {room.PricePerHour:C}/hr",
                    "Manage features",
                    "Delete");

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        await UpdateRoomProperty(roomId, "number", (r, value) => r.Number = value);
                        break;
                    case ConsoleKey.D2:
                        await UpdateRoomNumericProperty(roomId, "capacity", (r, value) => r.Capacity = (int)value);
                        break;
                    case ConsoleKey.D3:
                        await UpdateRoomNumericProperty(roomId, "price per hour", (r, value) => r.PricePerHour = value);
                        break;
                    case ConsoleKey.D4:
                        await ManageRoomFeatures(roomId);
                        break;
                    case ConsoleKey.D5:
                        var result = await service.DeleteRoomAsync(roomId);
                        SharedUIMethods.PrintResultMessage(result);
                        if (result.Success) return;
                        break;
                    case ConsoleKey.D0:
                        return;
                }
            }
        }

        private async Task UpdateRoomProperty(int roomId, string property, Action<ConferenceRoom, string> update)
        {
            ServiceResult result;
            do
            {
                Console.Write($"Enter new {property}: ");
                var input = Console.ReadLine()!;
                if (input == "0") return;

                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IConferenceRoomService>();
                var room = await service.GetRoomByIdAsync(roomId);
                if(room is null)
                {
                    SharedUIMethods.PrintMessagePause("Could not find room.");
                    return;
                }

                update(room!, input);
                result = await service.UpdateRoomAsync(room);

                if (!result.Success)
                {
                    foreach (var error in result.Errors)
                        Console.WriteLine($"  {error.Value}");
                }
            } while (!result.Success);

            SharedUIMethods.PrintResultMessage(result);
        }

        private async Task UpdateRoomNumericProperty(int roomId, string property, Action<ConferenceRoom, decimal> update)
        {
            while (true)
            {
                Console.Write($"Enter new {property}: ");
                if (!decimal.TryParse(Console.ReadLine(), out var value))
                {
                    Console.WriteLine($"Invalid {property}.");
                    continue;
                }
                if (value == 0) return;
                if (value > 99999)
                {
                    Console.WriteLine("Value is too large.");
                    continue;
                }

                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IConferenceRoomService>();
                var room = await service.GetRoomByIdAsync(roomId);
                if (room is null)
                {
                    SharedUIMethods.PrintMessagePause("Could not find room.");
                    return;
                }

                update(room!, value);
                var result = await service.UpdateRoomAsync(room);

                if (result.Success)
                {
                    SharedUIMethods.PrintResultMessage(result);
                    return;
                }

                foreach (var error in result.Errors)
                    Console.WriteLine($"  {error.Value}");
            }
        }

        private async Task CreateRoom()
        {
            Console.Write("Room number: ");
            var number = Console.ReadLine()!;

            int capacity;
            while (true)
            {
                Console.Write("Capacity: ");
                if (int.TryParse(Console.ReadLine(), out capacity) && capacity <= 9999)
                    break;
                Console.WriteLine("Invalid capacity.");
            }

            decimal price;
            while (true)
            {
                Console.Write("Price per hour: ");
                if (decimal.TryParse(Console.ReadLine(), out price) && price <= 99999)
                    break;
                Console.WriteLine("Invalid price.");
            }

            var room = new ConferenceRoom(number, capacity, price);

            ServiceResult result;
            do
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IConferenceRoomService>();
                result = await service.CreateRoomAsync(room);

                if (!result.Success)
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"  {error.Value}");

                        switch (error.Key)
                        {
                            case "Number":
                                Console.Write("New number: ");
                                room.Number = Console.ReadLine()!;
                                break;
                            case "Capacity":
                                while (true)
                                {
                                    Console.Write("New capacity: ");
                                    if (int.TryParse(Console.ReadLine(), out var c) && c <= 9999)
                                    {
                                        room.Capacity = c;
                                        break;
                                    }
                                    Console.WriteLine("Invalid capacity.");
                                }
                                break;
                            case "Price per hour":
                                while (true)
                                {
                                    Console.Write("New price: ");
                                    if (decimal.TryParse(Console.ReadLine(), out var p) && p <= 99999)
                                    {
                                        room.PricePerHour = p;
                                        break;
                                    }
                                    Console.WriteLine("Invalid price.");
                                }
                                break;
                        }
                    }
                }
            } while (!result.Success);

            SharedUIMethods.PrintResultMessage(result);
        }

        private async Task ManageRoomFeatures(int roomId)
        {
            while (true)
            {
                using var scope = scopeFactory.CreateScope();
                var roomService = scope.ServiceProvider.GetRequiredService<IConferenceRoomService>();
                var featureService = scope.ServiceProvider.GetRequiredService<IRoomFeatureService>();

                var room = await roomService.GetRoomByIdWithFeaturesAsync(roomId);
                if (room is null)
                {
                    SharedUIMethods.PrintMessagePause("Room not found.");
                    return;
                }

                var allFeatures = await featureService.GetAllFeaturesAsync();
                if(allFeatures.Count == 0)
                {
                    SharedUIMethods.PrintMessagePause("Could not find any features.");
                    return;
                }
                var currentFeatures = room.RoomFeatures;

                Console.Clear();
                Console.WriteLine($"Features for {room.Number}:\n");

                if (currentFeatures.Count != 0)
                {
                    foreach (var f in currentFeatures)
                        Console.WriteLine($"  - {f.Name}");
                }
                else
                {
                    Console.WriteLine("No features assigned.");
                }

                var key = Menu.Show("Manage features", "Back", "Add feature", "Remove feature");

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        var available = allFeatures.Where(f => !currentFeatures.Any(cf => cf.Id == f.Id)).ToList();
                        if (available.Count == 0)
                        {
                            SharedUIMethods.PrintMessagePause("No more features to add.");
                            break;
                        }

                        var addOptions = available.Select(f => f.Name).ToArray();
                        var addKey = Menu.Show("Add feature", "Cancel", addOptions);

                        if (addKey.Key == ConsoleKey.D0) break;

                        var addIndex = addKey.Key - ConsoleKey.D1;
                        if (addIndex >= 0 && addIndex < available.Count)
                        {
                            room.RoomFeatures.Add(available[addIndex]);
                            await roomService.UpdateRoomAsync(room);
                            SharedUIMethods.PrintMessagePause($"{available[addIndex].Name} added.");
                        }
                        else
                            SharedUIMethods.PrintMessageSleep("Invalid choice.");
                        break;

                    case ConsoleKey.D2:
                        if (currentFeatures.Count == 0)
                        {
                            SharedUIMethods.PrintMessagePause("No features to remove.");
                            break;
                        }

                        var removeOptions = currentFeatures.Select(f => f.Name).ToArray();
                        var removeKey = Menu.Show("Remove feature", "Cancel", removeOptions);

                        if (removeKey.Key == ConsoleKey.D0) break;

                        var removeIndex = removeKey.Key - ConsoleKey.D1;
                        if (removeIndex >= 0 && removeIndex < currentFeatures.Count)
                        {
                            room.RoomFeatures.Remove(currentFeatures[removeIndex]);
                            await roomService.UpdateRoomAsync(room);
                            SharedUIMethods.PrintMessagePause($"{currentFeatures[removeIndex].Name} removed.");
                        }
                        else
                            SharedUIMethods.PrintMessageSleep("Invalid choice.");
                        break;

                    case ConsoleKey.D0:
                        return;
                }
            }
        }

        private async Task ManageFeatures()
        {
            while (true)
            {
                Console.Clear();
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IRoomFeatureService>();
                var features = await service.GetAllFeaturesAsync();
                if(features.Count == 0)
                {
                    SharedUIMethods.PrintMessageSleep("Could not find any features.");
                    return;
                }

                var options = features.Select(f => f.Name);

                if (features.Count < 9)
                    options = options.Append("Create new feature");

                var key = Menu.Show("Manage Room Features", "Back", options.ToArray());

                if (key.Key == ConsoleKey.D0) return;

                var index = key.Key - ConsoleKey.D1;

                if (index == features.Count)
                    await CreateFeature();
                else if (index >= 0 && index < features.Count)
                    await ManageSingleFeature(features[index].Id);
                else
                    SharedUIMethods.PrintMessageSleep("Invalid choice.");
            }
        }

        private async Task ManageSingleFeature(int featureId)
        {
            while (true)
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IRoomFeatureService>();
                var feature = await service.GetFeatureByIdAsync(featureId);

                if (feature is null)
                {
                    SharedUIMethods.PrintMessagePause("Feature not found.");
                    return;
                }

                var key = Menu.Show($"Managing: {feature.Name}", "Back",
                    $"Name: {feature.Name}",
                    "Delete");

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        await UpdateFeatureName(featureId);
                        break;
                    case ConsoleKey.D2:
                        var result = await service.DeleteFeatureAsync(featureId);
                        SharedUIMethods.PrintResultMessage(result);
                        if (result.Success) return;
                        break;
                    case ConsoleKey.D0:
                        return;
                    default:
                        SharedUIMethods.PrintMessageSleep("Invalid choice");
                        break;
                }
            }
        }

        private async Task UpdateFeatureName(int featureId)
        {
            ServiceResult result;
            do
            {
                Console.Write("Enter new name: ");
                var input = Console.ReadLine()!;
                if (input == "0") return;

                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IRoomFeatureService>();
                var feature = await service.GetFeatureByIdAsync(featureId);
                if (feature is null)
                {
                    SharedUIMethods.PrintMessagePause("Could not find feature.");
                    return;
                }

                feature!.Name = input;
                result = await service.UpdateFeatureAsync(feature);

                if (!result.Success)
                {
                    foreach (var error in result.Errors)
                        Console.WriteLine($"  {error.Value}");
                }
            } while (!result.Success);

            SharedUIMethods.PrintResultMessage(result);
        }

        private async Task CreateFeature()
        {
            Console.Write("Feature name: ");
            var name = Console.ReadLine()!;

            var feature = new RoomFeature(name);

            ServiceResult result;
            do
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IRoomFeatureService>();
                result = await service.CreateFeatureAsync(feature);

                if (!result.Success)
                {
                    foreach (var error in result.Errors)
                        Console.WriteLine($"  {error.Value}");

                    Console.Write("New name: ");
                    feature.Name = Console.ReadLine()!;
                }
            } while (!result.Success);

            SharedUIMethods.PrintResultMessage(result);
        }

        internal async Task SeeStatistics()
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IStatisticsService>();
            var stats = await service.GetStatisticsAsync();

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Statistics\n");
            Console.ResetColor();

            Console.WriteLine($"Most booked room:    {stats.MostBookedRoom} ({stats.MostBookedRoomCount} bookings)");
            Console.WriteLine($"Most popular add-on: {stats.MostPopularAddon} ({stats.MostPopularAddonCount} bookings)");

            Console.WriteLine($"\n{"Room",-10} {"Revenue",12} {"Occupancy for week",20}");
            Console.WriteLine($"{new string('-', 45)}");

            foreach (var room in stats.RevenuePerRoom)
            {
                var occupancy = stats.OccupancyPerRoom.FirstOrDefault(o => o.RoomNumber == room.RoomNumber);
                Console.WriteLine($"{room.RoomNumber,-10} {room.Revenue,12:C0} {occupancy?.OccupancyPercent,18:F1} %");
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\nNote: Revenue may appear low relative to occupancy because employee bookings are free.\n");
            Console.ResetColor();

            SharedUIMethods.PrintMessagePause("");
        }
    }
}
