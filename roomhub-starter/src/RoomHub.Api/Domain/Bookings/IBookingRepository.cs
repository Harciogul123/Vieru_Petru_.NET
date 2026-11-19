namespace RoomHub.Api.Domain.Bookings;

// Write side only. New bookings are added through Room.Book, so there is no Add here.
public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    // R9: active bookings of one person whose startUtc falls on the given UTC day, in any room.
    Task<int> CountActiveForPersonOnDayAsync(string bookedBy, DateTime dayUtc, CancellationToken cancellationToken);
}
