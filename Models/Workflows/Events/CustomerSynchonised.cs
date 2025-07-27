using Models.Infrastructure;
using Models.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Workflows.Events
{
    public class CustomerSynchonised : IEventInfo
    {
        public string EventName => nameof(CustomerSynchonised);

        public string CustomerId { get; }

        public CustomerDocument Document { get; }

        public CustomerSynchonised(string customerId, CustomerDocument document)
        {
            CustomerId = customerId;
            Document = document;
        }

        public override string ToString() => string.Format($"Event='{EventName}', CustomerId='{CustomerId}'");
    }
}
