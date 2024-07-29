using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Events.Common;

namespace Shared.Events
{
    public class StockNotReservedEvent:IEvents
    {
        public Guid BuyerId { get; set; }
        public Guid OrderId { get; set; }
        public string Message { get; set; }

    }
}
