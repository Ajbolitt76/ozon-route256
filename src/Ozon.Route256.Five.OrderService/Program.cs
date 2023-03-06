using Grpc.Net.ClientFactory;
using Ozon.Route256.Five;
using Ozon.Route256.Five.OrderService;
using Ozon.Route256.Five.OrderService.Configuration;
using Ozon.Route256.Five.OrderService.DbClientBalancer;
using Ozon.Route256.Five.OrderService.GrpcServices;
using InterceptorRegistration = Grpc.Net.ClientFactory.InterceptorRegistration;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(x => x.Configure(builder.Configuration));

builder.Services.AddGrpc(options => { options.Interceptors.Add<LoggerInterceptor>(); });
builder.Services.AddGrpcReflection();
builder.Services.AddGrpcClient<SdService.SdServiceClient>(
    options =>
    {
        options.Address = new Uri("http://localhost:8903");
        options.InterceptorRegistrations.Add(
            new InterceptorRegistration(
                InterceptorScope.Client,
                sp =>
                {
                    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                    return new LoggerInterceptor(loggerFactory.CreateLogger<LoggerInterceptor>());
                }));
    });


builder.Services.AddSingleton<IDbStore, DbStore>();
builder.Services.AddHostedService<SdConsumerHostedService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGrpcReflectionService();
}

app.MapControllers();
app.MapGrpcService<OrdersGrpcService>();

app.Run();