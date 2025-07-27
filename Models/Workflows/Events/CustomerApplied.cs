using Models.Infrastructure;
using Models.Infrastructure.Events;

namespace Models.Workflows.Events
{
    public class CustomerApplied : IEventInfo
    {
        public string EventName => nameof(CustomerApplied);

        public string CustomerId { get; }

        public CustomerDocument Document { get; }

        public CustomerApplied(string customerId, CustomerDocument document)
        {
            CustomerId = customerId;
            Document = document;
        }

        public override string ToString() => string.Format($"Event='{EventName}', CustomerId='{CustomerId}'");
    }
}
