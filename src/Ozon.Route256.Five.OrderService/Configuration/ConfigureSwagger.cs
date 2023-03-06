using Microsoft.OpenApi.Models;

namespace Ozon.Route256.Five.OrderService.Configuration;

public static class ConfigureSwagger
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(
            c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrdersBackend", Version = "v1" });
            });
        
        return services;
    }

}