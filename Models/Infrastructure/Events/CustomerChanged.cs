namespace Models.Infrastructure.Events
{
    public class CustomerChanged : IEventInfo
    {
        public string EventName => nameof(CustomerChanged);

        public string CustomerId { get; }

        public CustomerDocument Document { get; }

        public CustomerChanged(string customerId, CustomerDocument document)
        {
            CustomerId = customerId;
            Document = document;
        }

        public override string ToString() => string.Format($"Event='{EventName}', CustomerId='{CustomerId}'");
    }
}
