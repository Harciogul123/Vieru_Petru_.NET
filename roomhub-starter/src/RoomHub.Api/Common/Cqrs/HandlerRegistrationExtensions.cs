using System.Reflection;

namespace RoomHub.Api.Common.Cqrs;

public static class HandlerRegistrationExtensions
{
    private static readonly Type[] HandlerContracts = [typeof(ICommandHandler<,>), typeof(IQueryHandler<,>)];

    // Reflection runs once, at startup. Every handler is registered as Scoped, like the DbContext it uses.
    public static IServiceCollection AddHandlers(this IServiceCollection services, Assembly assembly)
    {
        var implementations = assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false });

        foreach (var implementation in implementations)
        {
            var contracts = implementation.GetInterfaces()
                .Where(i => i.IsGenericType && HandlerContracts.Contains(i.GetGenericTypeDefinition()));

            foreach (var contract in contracts)
                services.AddScoped(contract, implementation);
        }

        return services;
    }
}
