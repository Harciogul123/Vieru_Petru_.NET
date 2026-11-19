using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public static class DateRangeErrors
{
    // R3
    public static readonly Error EndNotAfterStart =
        Error.Validation("DateRange.EndNotAfterStart", "endUtc must be strictly after startUtc.");
}
