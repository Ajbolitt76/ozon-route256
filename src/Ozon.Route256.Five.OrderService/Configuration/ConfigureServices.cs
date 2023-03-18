using Ozon.Route256.Five.OrderService.Services.DbClientBalancer;
using Ozon.Route256.Five.OrderService.Services.Domain;
using Ozon.Route256.Five.OrderService.Services.Repository;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddCoreServices(this IServiceCollection sc)
    {
        sc.AddSingleton<IDbStore, DbStore>();
        
        sc.AddHostedService<SdConsumerHostedService>();

        sc.AddSingleton<InMemoryStore>();
        sc.AddScoped<IRegionRepository, InMemoryRegionRepository>();
        sc.AddScoped<IOrderRepository, InMemoryOrderRepository>();
        sc.AddScoped<IDateTimeProvider, DateTimeProvider>();
        
        return sc;
    }
}