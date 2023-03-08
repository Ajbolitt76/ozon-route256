using Ozon.Route256.Five.OrderService;
using Ozon.Route256.Five.OrderService.Configuration;
using Ozon.Route256.Five.OrderService.GrpcServices;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(x => x.Configure(builder.Configuration));

builder.Services.AddGrpc(options => { options.Interceptors.Add<LoggerInterceptor>(); });
builder.Services.AddGrpcReflection();
builder.Services.AddGrpcClients(builder.Configuration);

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

app.Run();