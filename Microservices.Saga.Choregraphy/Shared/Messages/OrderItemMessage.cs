namespace Shared.Messages
{
    public class OrderItemMessage
    {
        //Stock API'nın ihtiyacı olan bilgiler
        public Guid ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}