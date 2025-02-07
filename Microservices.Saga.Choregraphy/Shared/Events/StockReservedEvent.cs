using Shared.Messages;

namespace Shared.Events
{
    public class StockReservedEvent
    {
        //Payment API'nın ihtiyacı olan bilgiler
        public Guid BuyerId { get; set; } //Ödeme yapan kişinin Id'si
        public Guid OrderId { get; set; } //Siparişin Id'si
        public decimal TotalPrice { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; } //Siparişteki ürünler
    }
}