var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/products", () => new[] { "Product 1", "Product 2", "Product 3" });

app.Run();