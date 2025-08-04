using Models.Infrastructure;
using Models.Infrastructure.Events;
using Models.Workflows.Events;

namespace Models.Workflows.Handlers
{
    public class EvaluationRequireDependencyHandler
    {
        public void Handle(IEventInfo evaluationRequireDependency)
        {
            var eventInfo = (EvaluationRequireDependency)evaluationRequireDependency;

            switch (eventInfo.TargetDependencyName)
            {
                case EntityName.Customer:
                    // Handle the case where the target dependency is a Customer
                    var customerDocument = Database.Instance.CustomerDocuments.First(c => c.Id == eventInfo.TargetDependencyId);

                    if (customerDocument.CurrentState == State.Evaluating)
                    {
                        // There is an ongoing evaluation for the customer. No action needed.
                    }

                    break;
            }
        }
    }
}
