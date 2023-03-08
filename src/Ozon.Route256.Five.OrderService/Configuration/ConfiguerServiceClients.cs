using Grpc.Core;
using Grpc.Net.ClientFactory;
using Ozon.Route256.Five.CustomersService.Grpc;
using Ozon.Route256.Five.LogisticsSimulator.Grpc;
using Ozon.Route256.Five.Sd.Grpc;

namespace Ozon.Route256.Five.OrderService.Configuration;

public static class ConfiguerServiceClients
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection sc, IConfiguration configuration)
    {
        AddGrpcClient<SdService.SdServiceClient>(sc, configuration);
        AddGrpcClient<LogisticsSimulatorService.LogisticsSimulatorServiceClient>(sc, configuration);
        AddGrpcClient<Customers.CustomersClient>(sc, configuration);

        return sc;
    }

    private static void AddGrpcClient<T>(IServiceCollection sc, IConfiguration configuration)
        where T : ClientBase
    {
        sc.AddGrpcClient<T>(
            options =>
            {
                var conStringName = $"{typeof(T).Name}Url";
                options.Address = new Uri( 
                    configuration.GetConnectionString(conStringName)
                    ?? throw new InvalidOperationException($"Не указан URL сервиса {typeof(T).FullName}. Ключ строки: {conStringName}"));
                options.InterceptorRegistrations.Add(
                    new InterceptorRegistration(
                        InterceptorScope.Client,
                        sp =>
                        {
                            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                            return new LoggerInterceptor(loggerFactory.CreateLogger<LoggerInterceptor>());
                        }));
            });
    }
}