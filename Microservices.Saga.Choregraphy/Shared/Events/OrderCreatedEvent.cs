using Shared.Messages;

namespace Shared.Events
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; } //Siparişin Id'si 
        public Guid BuyerId { get; set; } //Satın alan kisinin Id'si
        public decimal TotalPrice { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; } //Urunlerin listesi
    }
}