/* PaymentService - Program.cs */
var builder4 = WebApplication.CreateBuilder(args);
var app4 = builder4.Build();

app4.MapGet("/payments", () => new[] { "Payment 1", "Payment 2", "Payment 3" });

app4.Run();