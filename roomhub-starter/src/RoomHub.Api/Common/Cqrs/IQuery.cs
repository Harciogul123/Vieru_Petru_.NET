namespace RoomHub.Api.Common.Cqrs;

// Marker: a request that only reads; it never changes state.
public interface IQuery<TResult>
{
}
