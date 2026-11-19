namespace RoomHub.Api.Domain.Rooms;

// Write side only: only the methods the commands need.
public interface IRoomRepository
{
    // Loads the room together with its active bookings (needed by Room.Book for R7).
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    void Add(Room room);
}
