using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public static class BookingErrors
{
    // R2
    public static readonly Error BookedByRequired =
        Error.Validation("Booking.BookedByRequired", "bookedBy is required.");

    // R4
    public static readonly Error InvalidAttendees =
        Error.Validation("Booking.InvalidAttendees", "attendees must be strictly positive.");

    // R4
    public static readonly Error AttendeesExceedCapacity =
        Error.Validation("Booking.AttendeesExceedCapacity", "attendees cannot exceed the room capacity.");

    // R5
    public static readonly Error StartInPast =
        Error.Validation("Booking.StartInPast", "A booking cannot start in the past.");

    // R6
    public static readonly Error TooLong =
        Error.Validation("Booking.TooLong", "A booking can last at most 4 hours.");

    // R7
    public static readonly Error RoomOccupied =
        Error.Conflict("Booking.RoomOccupied", "The room already has an active booking in this interval.");

    // R9
    public static readonly Error DailyLimitReached =
        Error.Conflict("Booking.DailyLimitReached", "A participant can have at most 2 active bookings per day.");

    // R8
    public static readonly Error AlreadyCancelled =
        Error.Conflict("Booking.AlreadyCancelled", "The booking is already cancelled.");

    public static readonly Error NotFound =
        Error.NotFound("Booking.NotFound", "The booking does not exist.");
}
