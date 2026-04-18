using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ConferenceBooking.Infrastructure.Configurations
{
    public class ConferenceRoomConfiguration : IEntityTypeConfiguration<ConferenceRoom>
    {
        public void Configure(EntityTypeBuilder<ConferenceRoom> builder)
        {
            builder.Property(c => c.Number)
                .HasMaxLength(4);

            builder.Property(c => c.PricePerHour)
                .HasColumnType(SqlDbType.Money.ToString());

            builder.HasIndex(c => c.Number).IsUnique();

            builder.HasData(
                new ConferenceRoom { Id = 1, Number = "101", Capacity = 12, PricePerHour = 1500 },
                new ConferenceRoom { Id = 2, Number = "102", Capacity = 8, PricePerHour = 1000 },
                new ConferenceRoom { Id = 3, Number = "103", Capacity = 5, PricePerHour = 500 },
                new ConferenceRoom { Id = 4, Number = "104", Capacity = 30, PricePerHour = 3000 }
                );

            builder.HasMany(f => f.RoomFeatures)
                .WithMany(r => r.ConferenceRooms)
                .UsingEntity<Dictionary<string, object>>("ConferenceRoomFeature",
                f => f.HasOne<RoomFeature>().WithMany().HasForeignKey("RoomFeatureId").OnDelete(DeleteBehavior.Cascade),
                c => c.HasOne<ConferenceRoom>().WithMany().HasForeignKey("ConferenceRoomId").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("ConferenceRoomFeatures");
                    j.HasData(
                        new { ConferenceRoomId = 1, RoomFeatureId = 1},
                        new { ConferenceRoomId = 1, RoomFeatureId = 2},
                        new { ConferenceRoomId = 1, RoomFeatureId = 3},
                        new { ConferenceRoomId = 1, RoomFeatureId = 4},
                        new { ConferenceRoomId = 2, RoomFeatureId = 1},
                        new { ConferenceRoomId = 2, RoomFeatureId = 3},
                        new { ConferenceRoomId = 2, RoomFeatureId = 4},
                        new { ConferenceRoomId = 3, RoomFeatureId = 2},
                        new { ConferenceRoomId = 4, RoomFeatureId = 1}
                        );
                });
        }
    }
}
