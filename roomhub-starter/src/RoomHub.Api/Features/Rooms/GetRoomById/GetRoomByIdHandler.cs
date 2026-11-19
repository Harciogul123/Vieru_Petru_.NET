using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Infrastructure.Persistence;

namespace RoomHub.Api.Features.Rooms.GetRoomById;

// Read side: DbContext directly, AsNoTracking and a projection. Queries never change state.
public sealed class GetRoomByIdHandler(RoomHubDbContext db) : IQueryHandler<GetRoomByIdQuery, RoomDetails>
{
    public async Task<Result<RoomDetails>> Handle(GetRoomByIdQuery query, CancellationToken cancellationToken)
    {
        var room = await db.Rooms
            .AsNoTracking()
            .Where(r => r.Id == query.Id)
            .Select(r => new RoomDetails(r.Id, r.Name, r.Capacity))
            .FirstOrDefaultAsync(cancellationToken);

        if (room is null)
            return RoomErrors.NotFound;

        return room;
    }
}
