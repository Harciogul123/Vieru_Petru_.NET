namespace RoomHub.Api.Domain.Common;

// Used as Result<Unit> by operations that return no data.
public readonly record struct Unit
{
    public static readonly Unit Value = new();
}
