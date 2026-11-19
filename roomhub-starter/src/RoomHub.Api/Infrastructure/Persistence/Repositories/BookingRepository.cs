using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure.Persistence.Repositories;

public sealed class BookingRepository(RoomHubDbContext db) : IBookingRepository
{
    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Bookings.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public Task<int> CountActiveForPersonOnDayAsync(string bookedBy, DateTime dayUtc, CancellationToken cancellationToken)
    {
        var dayStart = DateTime.SpecifyKind(dayUtc.Date, DateTimeKind.Utc);
        var dayEnd = dayStart.AddDays(1);

        return db.Bookings.CountAsync(b =>
                b.BookedBy == bookedBy
                && b.Status == BookingStatus.Active
                && b.Range.StartUtc >= dayStart
                && b.Range.StartUtc < dayEnd,
            cancellationToken);
    }
}
