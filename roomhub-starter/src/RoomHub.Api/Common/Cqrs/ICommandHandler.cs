using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Common.Cqrs;

// One handler per command. No mediator library: the mechanism stays visible.
public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken);
}
