using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Order.API.Models.Enums;
using Shared.Events;

namespace Order.API.Consumers
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        private readonly OrderAPIDbContext _orderApiDbContext;

        public PaymentCompletedEventConsumer(OrderAPIDbContext orderApiDbContext)
        {
            _orderApiDbContext = orderApiDbContext;
        }
        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            Models.Entities.Order order = await _orderApiDbContext.Orders.FirstOrDefaultAsync(or => or.OrderId == context.Message.OrderId);

            order.OrderStatus = OrderStatus.Completed;

            await _orderApiDbContext.SaveChangesAsync();

        }
    }
}
