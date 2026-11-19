using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed class CreateBookingHandler(IRoomRepository rooms, IUnitOfWork unitOfWork, TimeProvider time)
    : ICommandHandler<CreateBookingCommand, CreateBookingResult>
{
    public async Task<Result<CreateBookingResult>> Handle(CreateBookingCommand command, CancellationToken cancellationToken)
    {
        // The contract: when the room does not exist, 404 comes before any validation.
        var room = await rooms.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
            return RoomErrors.NotFound;

        // The rules (R2-R7) live in Room.Book; the handler only orchestrates.
        var result = room.Book(
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            time.GetUtcNow().UtcDateTime);

        if (result.IsFailure)
            return result.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var booking = result.Value;
        return new CreateBookingResult(booking.Id, booking.RoomId, booking.Status);
    }
}
