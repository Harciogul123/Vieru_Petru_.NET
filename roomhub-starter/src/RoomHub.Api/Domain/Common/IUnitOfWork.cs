namespace RoomHub.Api.Domain.Common;

// DbContext.SaveChangesAsync already does this; the interface hides it behind a domain contract.
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
