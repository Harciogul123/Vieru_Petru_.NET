using RoomHub.Api.Common.Cqrs;

namespace RoomHub.Api.Features.Rooms.GetRoomById;

public sealed record GetRoomByIdQuery(Guid Id) : IQuery<RoomDetails>;

public sealed record RoomDetails(Guid Id, string Name, int Capacity);
