namespace Models.Workflows.Events
{
    public class EvaluationRequireDependency : IWorkflowEvent
    {
        public string EventName => nameof(EvaluationRequireDependency);
        public string EntityId { get; }
        public string EntityName { get; }

        public string TargetDependencyId { get; }
        public string TargetDependencyName { get; }

        public EvaluationRequireDependency(string entityId, string entityName, string targetDependencyId, string targetDepdencyName)
        {
            EntityId = entityId;
            EntityName = entityName;
            TargetDependencyId = targetDependencyId;
            TargetDependencyName = targetDepdencyName;
        }
    }
}
