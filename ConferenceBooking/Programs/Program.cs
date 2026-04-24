using ConferenceBooking.Application.RepositoryInterfaces;
using ConferenceBooking.Application.ServiceInterfaces;
using ConferenceBooking.Application.Services;
using ConferenceBooking.Infrastructure;
using ConferenceBooking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ConferenceBooking.Presentation.Programs
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //läs mer om detta i dokumentationen
            var services = new ServiceCollection(); //initialising servicecollection which dependencies (services and their interfaces) are added to

            services.AddDbContext<ConferenceBookingContext>(); //registering DbContext in servicecollection. provides services with a database connection

            services.AddScoped<IAddOnRepository, AddOnRepository>(); //registering all services and their interfaces.
            services.AddScoped<IAddOnService, AddOnService>(); //they are registered together and the code in the presentation layer is dependent on the abstraction of the service not the actual implementation

            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingService, BookingService>();

            services.AddScoped<IBookingAddOnRepository, BookingAddOnRepository>();

            services.AddScoped<IConferenceRoomRepository, ConferenceRoomRepository>();
            services.AddScoped<IConferenceRoomService, ConferenceRoomService>();

            services.AddScoped<IRoomFeatureRepository, RoomFeatureRepository>();
            services.AddScoped<IRoomFeatureService, RoomFeatureService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IStatisticsService, StatisticsService>();

            var provider = services.BuildServiceProvider(); //with buildserviceprovider the service container is created
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>(); //initializing scope factory to inject into classes

            await DataSeeder.SeedAsync(scopeFactory); //calling data seeder

            await new LogInProgram(scopeFactory).Run(); //injecting scopefactory into LogInProgram constructor
        }
    }
}
