using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Rooms;

public sealed class Room
{
    private readonly List<Booking> _bookings = [];

    // For EF Core.
    private Room()
    {
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int Capacity { get; private set; }

    public IReadOnlyCollection<Booking> Bookings => _bookings.AsReadOnly();

    // R1
    public static Result<Room> Create(string? name, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            return RoomErrors.NameRequired;

        if (capacity <= 0)
            return RoomErrors.InvalidCapacity;

        return new Room
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Capacity = capacity
        };
    }

    // R2-R7 and R9. nowUtc comes from outside (TimeProvider), so the domain never reads the clock.
    // The overlap check (R7) needs the room's active bookings to be loaded.
    // R9 needs data from all rooms, so the handler counts it through the repository and passes it in.
    public Result<Booking> Book(
        string? bookedBy,
        DateTime start,
        DateTime end,
        int attendees,
        DateTime nowUtc,
        int activeBookingsOfPersonThatDay)
    {
        if (string.IsNullOrWhiteSpace(bookedBy))
            return BookingErrors.BookedByRequired;

        var rangeResult = DateRange.Create(start, end);
        if (rangeResult.IsFailure)
            return rangeResult.Error;

        var range = rangeResult.Value;

        if (attendees <= 0)
            return BookingErrors.InvalidAttendees;

        if (attendees > Capacity)
            return BookingErrors.AttendeesExceedCapacity;

        if (range.StartUtc < nowUtc)
            return BookingErrors.StartInPast;

        if (range.Duration > Booking.MaxDuration)
            return BookingErrors.TooLong;

        if (_bookings.Any(b => b.IsActive && b.Range.Overlaps(range)))
            return BookingErrors.RoomOccupied;

        // R9
        if (activeBookingsOfPersonThatDay >= Booking.MaxActivePerPersonPerDay)
            return BookingErrors.DailyLimitReached;

        var booking = Booking.Create(Id, bookedBy.Trim(), range, attendees);
        _bookings.Add(booking);
        return booking;
    }
}
