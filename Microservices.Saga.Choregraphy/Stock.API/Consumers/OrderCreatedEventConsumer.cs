using MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.Events;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer(MongoDBService mongoDBService, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint) : IConsumer<OrderCreatedEvent>
    {
        //readonly MongoDBService _mongoDBService;

        //public OrderCreatedEventConsumer(MongoDBService mongoDBService)
        //{
        //    _mongoDBService = mongoDBService;
        //}

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            IMongoCollection<Models.Stock> collection = mongoDBService.GetCollection<Models.Stock>();

            foreach (var orderItem in context.Message.OrderItems)
            {
                //Siparişteki her bir ürün için stok kontrolü yapılacak...
                stockResult.Add(await (await collection.FindAsync(s => s.ProductId == orderItem.ProductId.ToString() && s.Count > (long)orderItem.Count)).AnyAsync());
            }

            if (stockResult.TrueForAll(s => s.Equals(true))) //Eğer tüm stocklar yeterli ise...
            {
                //Stock güncellemesi...
                foreach (var orderItem in context.Message.OrderItems)
                {
                    Models.Stock stock = await (await collection.FindAsync(s => s.ProductId == orderItem.ProductId.ToString())).FirstOrDefaultAsync();
                    stock.Count -= orderItem.Count; //Stoktan düşülmesi

                    await collection.FindOneAndReplaceAsync(x => x.ProductId == orderItem.ProductId.ToString(), stock);
                }
                //Payment'ı uyaracak event'in fırlatılması...
                //Payment API'ya StockReservedEvent gönderilecek...
                var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Payment_StockReservedEventQueue}"));
                StockReservedEvent stockReservedEvent = new()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    TotalPrice = context.Message.TotalPrice,
                    OrderItems = context.Message.OrderItems,
                };
                await sendEndpoint.Send(stockReservedEvent);
            }
            else
            {
                //Stok işlemi başarısız...
                //Order'ı uyaracak event fırlatılacaktır.
                //Order API'ya StockNotReservedEvent gönderilecek...
                StockNotReservedEvent stockNotReservedEvent = new()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    Message = "Stok miktarı yetersiz..."
                };

                await publishEndpoint.Publish(stockNotReservedEvent);
            }
        }
    }
}