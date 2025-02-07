using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models.Contexts;
using Order.API.ViewModels;
using Shared.Events;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrderAPIDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLServer")));
builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ"]);
    });
});

var app = builder.Build();

app.MapPost("/create-order", async (CreateOrderVM model, OrderAPIDbContext context, IPublishEndpoint publishEndpoint) =>
{
    Order.API.Models.Order order = new()
    {
        BuyerId = Guid.TryParse(model.BuyerId, out Guid _buyerId) ? _buyerId : Guid.NewGuid(), //Guide döünüþütürülebilirse dönüþtür yoksa yeni bir Guid oluþtur
        OrderItems = model.OrderItems.Select(oi => new Order.API.Models.OrderItem()
        {
            Count = oi.Count,
            Price = oi.Price,
            ProductId = Guid.Parse(oi.ProductId)
        }).ToList(),

        OrderStatus = Order.API.Enums.OrderStatus.Suspend,  //Ilk oluþturulduðunda Suspend durumunda olacak
        CreatedDate = DateTime.UtcNow, //Oluþturulma tarihi
        TotalPrice = model.OrderItems.Sum(oi => oi.Price * oi.Count) //Toplam fiyatý hesapla
    };

    await context.Orders.AddAsync(order);
    await context.SaveChangesAsync();

    OrderCreatedEvent orderCreatedEvent = new()
    {
        BuyerId = order.BuyerId,
        OrderId = order.Id,
        TotalPrice = order.TotalPrice,
        OrderItems = order.OrderItems.Select(oi => new Shared.Messages.OrderItemMessage()
        {
            Count = oi.Count,
            Price = oi.Price,
            ProductId = oi.ProductId,
        }).ToList()
    };
    await publishEndpoint.Publish(orderCreatedEvent); //OrderCreatedEvent'i publish et
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();