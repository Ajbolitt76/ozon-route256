using System.Runtime.Intrinsics.X86;

namespace Ozon.Route256.Five.OrderService.Cqrs;

public static class CqrsServiceCollectionExtensions
{
    public static IServiceCollection AddCqrs(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICommandDispatcher, CommandDispatcher>();
        serviceCollection.AddScoped<IQueryDispatcher, QueryDispatcher>();

        return serviceCollection;
    }

    public static IServiceCollection AddCqrsHandlers(this IServiceCollection serviceCollection, params Type[] types)
    {
        var allowedTypes = new[]
        {
            typeof(ICommandHandler<,>),
            typeof(ICommandHandler<>),
            typeof(IQueryHandler<,>),
            typeof(IRequestHandler<,>),
            typeof(IRequestHandler)
        };

        serviceCollection.Scan(
            x =>
                x.FromAssembliesOf(types)
                    .AddClasses(
                        x =>
                            x.AssignableToAny(allowedTypes))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());

        return serviceCollection;
    }
}