/* CustomerService - Program.cs */
var builder2 = WebApplication.CreateBuilder(args);
var app2 = builder2.Build();

app2.MapGet("/customers", () => new[] { "Customer 1", "Customer 2", "Customer 3" });

app2.Run();