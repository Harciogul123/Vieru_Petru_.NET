using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Infrastructure.Persistence.Configurations;

public sealed class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");
        builder.HasKey(r => r.Id);

        // The Id is generated in the domain, not in the database.
        builder.Property(r => r.Id).ValueGeneratedNever();
        builder.Property(r => r.Name).IsRequired();
        builder.Property(r => r.Capacity).IsRequired();

        builder.HasMany(r => r.Bookings)
            .WithOne()
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        // EF writes the private _bookings list, not the read-only property.
        builder.Navigation(r => r.Bookings)
            .HasField("_bookings")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
