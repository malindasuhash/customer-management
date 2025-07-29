using Models.Infrastructure;
using Models.Infrastructure.Events;

namespace Models.Workflows.Events
{
    public class CustomerAppliedEvent : IEventInfo
    {
        public string EventName => nameof(CustomerAppliedEvent);

        public string CustomerId { get; }

        public CustomerDocument Document { get; }

        public CustomerAppliedEvent(string customerId, CustomerDocument document)
        {
            CustomerId = customerId;
            Document = document;
        }

        public override string ToString() => string.Format($"Event='{EventName}', CustomerId='{CustomerId}'");
    }
}
