using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Rooms;

public static class RoomErrors
{
    // R1
    public static readonly Error NameRequired =
        Error.Validation("Room.NameRequired", "The room name is required.");

    // R1
    public static readonly Error InvalidCapacity =
        Error.Validation("Room.InvalidCapacity", "The capacity must be strictly positive.");

    public static readonly Error NotFound =
        Error.NotFound("Room.NotFound", "The room does not exist.");
}
