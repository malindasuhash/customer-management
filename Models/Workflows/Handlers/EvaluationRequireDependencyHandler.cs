using Models.Infrastructure;
using Models.Workflows.Events;

namespace Models.Workflows.Handlers
{
    public class EvaluationRequireDependencyHandler
    {
        public void Handle(EvaluationRequireDependency evaluationRequireDependency)
        {
            switch (evaluationRequireDependency.EntityName)
            {
                case EntityName.LegalEntity:
                    var legalEntityDocument = Database.Instance.LegalEntityDocuments.First(l => l.Id == evaluationRequireDependency.EntityId);

                    // Transition the legal entity document to the next state
                    DocumentStateManager.Instance.Transition(legalEntityDocument);

                    // This is where things going to get interesting.
                    switch (evaluationRequireDependency.TargetDependencyName)
                    {
                        case EntityName.Customer:
                            // Handle the case where the target dependency is a Customer
                            var customerDocument = Database.Instance.CustomerDocuments.First(c => c.Id == evaluationRequireDependency.TargetDependencyId);

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
