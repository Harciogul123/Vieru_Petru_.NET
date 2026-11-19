using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Features.Bookings.GetBookingById;

public sealed record GetBookingByIdQuery(Guid Id) : IQuery<BookingDetails>;

public sealed record BookingDetails(
    Guid Id,
    Guid RoomId,
    string BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees,
    BookingStatus Status);
