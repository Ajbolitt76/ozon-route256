using Ozon.Route256.Five.OrderService;
using Ozon.Route256.Five.OrderService.Configuration;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Services.Database;
using Ozon.Route256.Five.OrderService.Services.Database.Migrator;
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
    .AddDatabase(builder.Configuration)
    .AddCoreServices(builder.Configuration);

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

await PerformMigrations();

app.Run();

async Task PerformMigrations()
{
    var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (s, e) =>
    {
        cts.Cancel();
        e.Cancel = true;
    };

    await using var scope = app!.Services.CreateAsyncScope();
    var sp = scope.ServiceProvider;
    var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Migrator");
    var runner = sp.GetRequiredService<InServiceShardedMigrator>();

    logger.LogInformation("Проводим миграции...");

    try
    {
        await runner.Migrate(cts.Token);
    }
    catch (Exception e)
    {
        logger.LogCritical("Не удалось провести миграции", e);
        throw;
    }
}