using Shared.Events.Common;
using Shared.Messages;

namespace Shared.Events
{
    public class OrderCreatedEvent:IEvents
    {
        public Guid OrderId { get; set; }
        public Guid BuyerId { get; set; }
        public List<OrderItemMessage> OrderItems{get;set;}
        public decimal TotalPrice { get; set; }
    }
}
