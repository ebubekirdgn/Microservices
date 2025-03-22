/* OrderService - Program.cs */
var builder3 = WebApplication.CreateBuilder(args);
var app3 = builder3.Build();

app3.MapGet("/orders", () => new[] { "Order 1", "Order 2", "Order 3" });

app3.Run();