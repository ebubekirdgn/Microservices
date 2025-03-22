using Steeltoe.Common.Discovery;
using Steeltoe.Discovery;
using Steeltoe.Discovery.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDiscoveryClient(builder.Configuration);
var app = builder.Build();

app.MapGet("/", async (IDiscoveryClient discoveryClient) =>
{
    #region Service Discovery Olmaksýzýn Yaþanan Kepazelik Durumu
    //HttpClient httpClient = new();
    //var response = await httpClient.GetAsync("https://localhost:7047");
    //var result = await response.Content.ReadAsStringAsync();
    //return Results.Ok(result);
    #endregion
    #region Service Discovery Ýle Ýdeal Yaklaþým
    DiscoveryHttpClientHandler discoveryHttpClientHandler = new(discoveryClient);
    using HttpClient httpClient = new(discoveryHttpClientHandler, false);
    return await httpClient.GetStringAsync("https://servicec");
    #endregion
});

app.Run();
