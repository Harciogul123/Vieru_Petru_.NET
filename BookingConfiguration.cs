using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure.Persistence.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(b => b.Id);

        // Important: the Id is set in the domain. Without this, a booking added through
        // Room.Book would be treated as an existing row (UPDATE instead of INSERT).
        builder.Property(b => b.Id).ValueGeneratedNever();
        builder.Property(b => b.BookedBy).IsRequired();
        builder.Property(b => b.Attendees).IsRequired();

        // Enums are stored as text, so the database stays readable.
        builder.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // DateRange lives in the Bookings table, no separate table.
        builder.ComplexProperty(b => b.Range, range =>
        {
            range.Property(r => r.StartUtc)
                .HasColumnName("StartUtc")
                .HasConversion<UtcDateTimeConverter>();
            range.Property(r => r.EndUtc)
                .HasColumnName("EndUtc")
                .HasConversion<UtcDateTimeConverter>();
        });

        builder.HasIndex(b => b.RoomId);
    }
}
