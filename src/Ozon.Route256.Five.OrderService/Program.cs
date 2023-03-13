using System.Runtime.ExceptionServices;
using Ozon.Route256.Five.OrderService;
using Ozon.Route256.Five.OrderService.Configuration;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.GrpcServices;
using Ozon.Route256.Five.OrderService.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(x => x.Configure(builder.Configuration));

builder.Services.AddGrpc(options => { options.Interceptors.Add<LoggerInterceptor>(); });
builder.Services.AddGrpcReflection();
builder.Services.AddGrpcClients(builder.Configuration);

builder.Services.AddCqrs();
builder.Services.AddCoreServices();

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

try
{
    app.Logger.LogInformation("Начинаем инициализацю inmemory хранилища...");
    var store = app.Services.GetRequiredService<InMemoryStore>();
    await store.FillData();
    app.Logger.LogInformation("Инициализаця inmemory хранилища законченна...");
}
catch (Exception e)
{
    app.Logger.LogCritical(e, "Невозможно инициализировать данные для сервиса...");
    throw;
}

app.Run();