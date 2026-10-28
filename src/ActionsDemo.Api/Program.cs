using GreetingService;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/greet", (string? name) => Results.Ok(new { greeting = Greeter.Greet(name) }));

app.MapGet("/greet/{language}", (string language, string? name) =>
    Results.Ok(new { greeting = Greeter.GreetIn(name, language) }));

app.Run();
