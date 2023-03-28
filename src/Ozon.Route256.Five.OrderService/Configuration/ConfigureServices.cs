using FluentMigrator.Runner;
using Ozon.Route256.Five.OrderService.Migrations;
using Ozon.Route256.Five.OrderService.Services.Database;
using Ozon.Route256.Five.OrderService.Services.DbClientBalancer;
using Ozon.Route256.Five.OrderService.Services.Domain;
using Ozon.Route256.Five.OrderService.Services.MicroserviceClients;
using Ozon.Route256.Five.OrderService.Services.Repository;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddCoreServices(this IServiceCollection sc, IConfiguration configuration)
    {
        sc.AddSingleton<IDbStore, DbStore>();
        
        sc.AddHostedService<SdConsumerHostedService>();

        sc.AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .AddPostgres()
                .WithGlobalConnectionString(configuration.GetConnectionString("DbConnection"))
                .WithMigrationsIn(typeof(InitialMigration).Assembly)
            );

        sc.AddScoped(x => 
            new DatabaseConnectionString(
                configuration.GetConnectionString("DbConnection")
                ?? throw new InvalidOperationException("Не указанно подключение к БД")));

        sc.AddSingleton<InMemoryStore>();
        sc.AddScoped<IRegionRepository, InMemoryRegionRepository>();
        sc.AddScoped<IOrderRepository, DbOrderRepository>();
        sc.AddScoped<IDateTimeProvider, DateTimeProvider>();
        sc.AddScoped<ICachedCustomersClient, CachedCustomersClient>();
        sc.AddScoped<IConnectionFactory, ConnectionFactory>();

        return sc;
    }
}