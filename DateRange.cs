using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

// Value object: it can only be created valid (R3).
public sealed record DateRange
{
    // For EF Core.
    private DateRange()
    {
    }

    public DateTime StartUtc { get; private set; }

    public DateTime EndUtc { get; private set; }

    public TimeSpan Duration => EndUtc - StartUtc;

    public static Result<DateRange> Create(DateTime start, DateTime end)
    {
        var startUtc = ToUtc(start);
        var endUtc = ToUtc(end);

        if (endUtc <= startUtc)
            return DateRangeErrors.EndNotAfterStart;

        return new DateRange { StartUtc = startUtc, EndUtc = endUtc };
    }

    // Half-open intervals [start, end): ranges that only touch do not overlap (R7).
    public bool Overlaps(DateRange other) =>
        StartUtc < other.EndUtc && other.StartUtc < EndUtc;

    // The contract says dates are UTC; a value without a Kind is treated as UTC, not as local time.
    private static DateTime ToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
