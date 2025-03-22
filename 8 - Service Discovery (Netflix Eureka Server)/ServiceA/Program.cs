using Steeltoe.Common.Discovery;
using Steeltoe.Discovery;
using Steeltoe.Discovery.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDiscoveryClient(builder.Configuration);
builder.Services
    .AddHttpClient("serviceb", options => options.BaseAddress = new Uri("https://serviceb"))
    .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
    {
        var discoveryClient = serviceProvider.GetService<IDiscoveryClient>();
        return new DiscoveryHttpClientHandler(discoveryClient);
    });


var app = builder.Build();

app.MapGet("/", async (/*IDiscoveryClient discoveryClient*/ IHttpClientFactory httpClientFactory) =>
{
    #region Service Discovery Olmaksýzýn Yaþanan Kepazelik Durumu
    //HttpClient httpClient = new();
    //var response = await httpClient.GetAsync("https://localhost:7098");
    //var result = await response.Content.ReadAsStringAsync();
    //return Results.Ok(result);
    #endregion
    #region Service Discovery Ýle Ýdeal Yaklaþým - Ameleus
    //DiscoveryHttpClientHandler discoveryHttpClientHandler = new(discoveryClient);
    //using HttpClient httpClient = new(discoveryHttpClientHandler, false);
    //return await httpClient.GetStringAsync("https://serviceb");
    #endregion
    #region  Service Discovery Ýle Ýdeal Yaklaþým - Best Practices
    var httpClient = httpClientFactory.CreateClient("serviceb");
    return await httpClient.GetStringAsync("/");
    #endregion
});

app.Run();
