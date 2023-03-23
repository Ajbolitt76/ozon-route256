using Ozon.Route256.Five.OrderService;
using Ozon.Route256.Five.OrderService.Configuration;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Services.GrpcServices;
using Ozon.Route256.Five.OrderService.Services.Kafka;
using Ozon.Route256.Five.OrderService.Services.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(x => x.Configure(builder.Configuration));

builder.Services.AddGrpc(options => { options.Interceptors.Add<LoggerInterceptor>(); });
builder.Services
    .AddGrpcReflection()
    .AddGrpcClients(builder.Configuration);

builder.Services
    .AddCqrs()
    .AddRedis(builder.Configuration)
    .AddKafka(builder.Configuration)
    .AddCoreServices();

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