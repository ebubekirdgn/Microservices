using MassTransit;
using Order.API.Context;
using Shared.OrderEvents;

namespace Order.API.Consumers
{
    public class OrderCompletedEventConsumer(OrderDbContext orderDbContext) : IConsumer<OrderCompletedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
        {
            //Burada OrderCompletedEvent mesajını alıp OrderId'si ile Order tablosundaki ilgili siparişi bulup durumunu Completed yapacağız.
            Order.API.Models.Order order = await orderDbContext.Orders.FindAsync(context.Message.OrderId);
            if (order != null)
            {
                order.OrderStatus = Enums.OrderStatus.Completed;
                await orderDbContext.SaveChangesAsync();
            }
        }
    }
}