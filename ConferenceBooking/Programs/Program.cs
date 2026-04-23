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
            var services = new ServiceCollection();

            services.AddDbContext<ConferenceBookingContext>();

            services.AddScoped<IAddOnRepository, AddOnRepository>();
            services.AddScoped<IAddOnService, AddOnService>();

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

            var provider = services.BuildServiceProvider();
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            await DataSeeder.SeedAsync(scopeFactory);

            await new LogInProgram(scopeFactory).Run();
        }
    }
}
