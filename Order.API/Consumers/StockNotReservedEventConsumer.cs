using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Order.API.Models.Enums;
using Shared.Events;

namespace Order.API.Consumers
{
    public class StockNotReservedEventConsumer : IConsumer<StockNotReservedEvent>
    {
        private readonly OrderAPIDbContext _orderApiDbContext;

        public StockNotReservedEventConsumer(OrderAPIDbContext orderApiDbContext)
        {
            _orderApiDbContext = orderApiDbContext;
        }

        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            Models.Entities.Order order = await _orderApiDbContext.Orders.FirstOrDefaultAsync(or => or.OrderId == context.Message.OrderId);

            order.OrderStatus = OrderStatus.Failed;

            await _orderApiDbContext.SaveChangesAsync();
        }
    }
}
