using Models.Infrastructure.Events;

namespace Models.Workflows.Events
{
    public class EvaluationFailedEvent : IWorkflowEvent
    {
        public string EventName => nameof(EvaluationFailedEvent);

        public string EntityId { get; }
        public string EntityName { get; }
        public string Reason { get; }

        public EvaluationFailedEvent(string entityId, string entityName, string reason)
        {
            EntityId = entityId;
            EntityName = entityName;
            Reason = reason;
        }
    }
}
