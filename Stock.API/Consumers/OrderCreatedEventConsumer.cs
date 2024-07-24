using MassTransit;
using Shared.Events;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        public  Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            Console.WriteLine(context.Message.BuyerId + "---" + context.Message.OrderId );
            return Task.CompletedTask;
        }
    }
}
