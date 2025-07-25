using Models.Infrastructure.Events;

namespace Models.Workflows.Events
{
    public class EvaluationCompleteEvent : IWorkflowEvent
    {
        public string EventName => nameof(EvaluationCompleteEvent);
        public string EntityId { get; }
        public string EntityName { get; }
        public EvaluationCompleteEvent(string entityId, string entityName)
        {
            EntityId = entityId;
            EntityName = entityName;
        }
    }
}
