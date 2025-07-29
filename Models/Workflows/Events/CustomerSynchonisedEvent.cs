using Models.Infrastructure;
using Models.Infrastructure.Events;

namespace Models.Workflows.Events
{
    public class CustomerSynchonisedEvent : IEventInfo
    {
        public string EventName => nameof(CustomerSynchonisedEvent);

        public string CustomerId { get; }

        public CustomerDocument Document { get; }

        public CustomerSynchonisedEvent(string customerId, CustomerDocument document)
        {
            CustomerId = customerId;
            Document = document;
        }

        public override string ToString() => string.Format($"Event='{EventName}', CustomerId='{CustomerId}'");
    }
}
