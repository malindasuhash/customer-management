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

            switch (eventInfo.EntityName)
            {
                case EntityName.LegalEntity:
                    var legalEntityDocument = Database.Instance.LegalEntityDocuments.First(l => l.Id == eventInfo.EntityId);

                    // Transition the legal entity document to the next state
                    DocumentStateManager.Instance.Transition(legalEntityDocument);

                    // This is where things going to get interesting.
                    switch (eventInfo.TargetDependencyName)
                    {
                        case EntityName.Customer:
                            // Handle the case where the target dependency is a Customer
                            var customerDocument = Database.Instance.CustomerDocuments.First(c => c.Id == eventInfo.TargetDependencyId);

                            if (customerDocument.Submitted != null)
                            {
                                // There is an ongoing evaluation for the customer. No action needed.
                            }

                            break;
                    }

                    break;
            }
        }
    }
}
