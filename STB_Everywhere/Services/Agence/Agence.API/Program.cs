var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

app.MapGet("/", () => "Hello World!");

app.Run();
