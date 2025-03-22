/* NginxManager - Program.cs */
var builder5 = WebApplication.CreateBuilder(args);
var app5 = builder5.Build();

app5.MapGet("/nginx/reload", () => {
    System.Diagnostics.Process.Start("nginx.exe", "-s reload");
    return Results.Ok("NGINX Reloaded");
});

app5.Run();