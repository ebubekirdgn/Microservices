namespace Shared.Events
{
    public class StockNotReservedEvent
    {
        //Order API'nın ihtiyacı olan bilgiler Stokta sorun olduğunda Order API'ya bildirilecek.
        public Guid OrderId { get; set; }
        public Guid BuyerId { get; set; }
        public string Message { get; set; }
    }
}