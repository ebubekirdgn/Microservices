namespace Shared.Events
{
    public class PaymentCompletedEvent
    {
        public Guid OrderId { get; set; } //Ödeme başarılı olduğunda tetiklenecek event
    }
}