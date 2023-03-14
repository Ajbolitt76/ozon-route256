using Ozon.Route256.Five.OrderService.Features.GetAllCustomers;

namespace Ozon.Route256.Five.OrderService.Cqrs;

public static class CqrsServiceCollectionExtensions
{
    public static IServiceCollection AddCqrs(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICommandDispatcher, CommandDispatcher>();
        serviceCollection.AddScoped<IQueryDispatcher, QueryDispatcher>();
        serviceCollection.AddCqrsHandlers(typeof(GetAllCustomerQueryHandler));
        
        return serviceCollection;
    }

    private static IServiceCollection AddCqrsHandlers(this IServiceCollection serviceCollection, params Type[] types)
    {
        var allowedTypes = new[]
        {
            typeof(ICommandHandler<,>),
            typeof(ICommandHandler<>),
            typeof(IQueryHandler<,>),
            typeof(IRequestHandler<,>),
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