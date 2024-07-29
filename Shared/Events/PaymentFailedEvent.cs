using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Events.Common;

namespace Shared.Events
{
    public class PaymentFailedEvent : IEvents
    {
        public Guid OrderId { get; set; }
        public string Message { get; set; }

    }
}
