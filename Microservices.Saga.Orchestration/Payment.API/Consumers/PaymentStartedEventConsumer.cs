using MassTransit;
using Shared.PaymentEvents;
using Shared.Settings;

namespace Payment.API.Consumers
{
    public class PaymentStartedEventConsumer(ISendEndpointProvider sendEndpointProvider) : IConsumer<PaymentStartedEvent>
    {
        //Burada ödeme başlatıldığında yapılacak işlemler yazılacak
        public async Task Consume(ConsumeContext<PaymentStartedEvent> context)
        {
            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));
            if (true)
            {

                PaymentCompletedEvent paymentCompletedEvent = new(context.Message.CorrelationId)
                {

                };

                await sendEndpoint.Send(paymentCompletedEvent);
            }
            else
            {
                PaymentFailedEvent paymentFailedEvent = new(context.Message.CorrelationId)
                {
                    Message = "Yetersiz bakiye...",
                    OrderItems = context.Message.OrderItems
                };
                //Ödeme başarısız olduğunda ödeme başarısız eventi gönderilecek
                await sendEndpoint.Send(paymentFailedEvent);
            }
        }
    }
}
