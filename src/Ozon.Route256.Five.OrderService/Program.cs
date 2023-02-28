var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Console.WriteLine("Hello");
app.MapGet("/", () => "Hello World!");

app.Run();  