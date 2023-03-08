using Ozon.Route256.Five.OrderService.DbClientBalancer;

namespace Ozon.Route256.Five.OrderService.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddCoreServices(this IServiceCollection sc)
    {
        sc.AddSingleton<IDbStore, DbStore>();
        
        sc.AddHostedService<SdConsumerHostedService>();

        return sc;
    }
}