using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public sealed class Booking
{
    // R6: exactly 4 hours is allowed.
    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4);

    // R9: at most 2 active bookings per bookedBy per UTC day of startUtc, across all rooms.
    public const int MaxActivePerPersonPerDay = 2;

    // R10: rooms with a capacity greater than LargeRoomCapacity can be booked for at most 2 hours.
    public const int LargeRoomCapacity = 10;

    public static readonly TimeSpan MaxDurationLargeRoom = TimeSpan.FromHours(2);

    // R11: a booking can be cancelled only if it starts at least 60 minutes from now.
    public static readonly TimeSpan MinNoticeForCancel = TimeSpan.FromMinutes(60);

    // For EF Core.
    private Booking()
    {
    }

    public Guid Id { get; private set; }

    public Guid RoomId { get; private set; }

    public string BookedBy { get; private set; } = string.Empty;

    public DateRange Range { get; private set; } = null!;

    public int Attendees { get; private set; }

    public BookingStatus Status { get; private set; }

    public bool IsActive => Status == BookingStatus.Active;

    // Internal: a booking is created only through Room.Book, which checks the rules.
    internal static Booking Create(Guid roomId, string bookedBy, DateRange range, int attendees) => new()
    {
        Id = Guid.NewGuid(),
        RoomId = roomId,
        BookedBy = bookedBy,
        Range = range,
        Attendees = attendees,
        Status = BookingStatus.Active
    };

    // R8: Active -> Cancelled; a cancelled booking no longer counts for R7/R9, so its interval is freed.
    // R11: nowUtc comes from TimeProvider, through the handler.
    public Result<Unit> Cancel(DateTime nowUtc)
    {
        if (Status == BookingStatus.Cancelled)
            return BookingErrors.AlreadyCancelled;

        if (Range.StartUtc - nowUtc < MinNoticeForCancel)
            return BookingErrors.TooLateToCancel;

        Status = BookingStatus.Cancelled;
        return Unit.Value;
    }
}
