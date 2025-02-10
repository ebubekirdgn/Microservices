using MassTransit;
using MongoDB.Driver;
using Shared.Messages;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class StockRollbackMessageConsumer(MongoDbService mongoDbService) : IConsumer<StockRollbackMessage>
    {
        public async Task Consume(ConsumeContext<StockRollbackMessage> context)
        {
            //Burada StockRollbackMessage mesajını alıp OrderItems'larındaki ürünlerin stoklarını arttıracağız.
            var stockCollection = mongoDbService.GetCollection<Stock.API.Models.Stock>();

            //Eğer tüm ürünlerin stokları yeterli ise stokları düşüreceğiz ve StockReservedEvent mesajını göndereceğiz.
            foreach (var orderItem in context.Message.OrderItems)
            {
                var stock = await (await stockCollection.FindAsync(x => x.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();

                stock.Count += orderItem.Count;
                await stockCollection.FindOneAndReplaceAsync(x => x.ProductId == orderItem.ProductId, stock);
            }
        }
    }
}