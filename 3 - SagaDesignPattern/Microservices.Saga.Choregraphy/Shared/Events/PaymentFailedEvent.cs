using Shared.Messages;

namespace Shared.Events
{
    public class PaymentFailedEvent
    {
        //Ödeme başarısız olduğunda tetiklenecek event
        public Guid OrderId { get; set; }

        public string Message { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; } //Siparişteki ürünler
    }
}