using MassTransit;
using Shared.Events;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumers : IConsumer<StockReservedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumers(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public  Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            //odeme islemleri ...

            if (true)
            {
                //odemenin basariyla tamamlandigini ifade etmemiz gerekiyor

                PaymentCompletedEvent paymentCompletedEvent = new()
                {
                    OrderId = context.Message.OrderId
                };

                _publishEndpoint.Publish(paymentCompletedEvent);
                Console.WriteLine("Odeme Basarili");
            }
            else
            {
                //odemede sikinti oludugunu belritmemiz gerekiyor

                PaymentFailedEvent paymentFailedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    Message = "bakiye yetersiz"
                };

                _publishEndpoint.Publish(paymentFailedEvent);
                
                Console.WriteLine("Odeme Basarisiz");
            }

            return Task.CompletedTask;
        }
    }
}
