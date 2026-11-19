using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Infrastructure.Persistence.Repositories;

public sealed class RoomRepository(RoomHubDbContext db) : IRoomRepository
{
    // Only the active bookings are loaded: those are the only ones that matter for R7.
    public Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Rooms
            .Include(r => r.Bookings.Where(b => b.Status == BookingStatus.Active))
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    // Add only marks the entity; IUnitOfWork saves.
    public void Add(Room room) => db.Rooms.Add(room);
}
