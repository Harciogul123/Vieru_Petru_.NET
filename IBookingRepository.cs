namespace RoomHub.Api.Domain.Bookings;

// Write side only. New bookings are added through Room.Book, so there is no Add here.
public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
