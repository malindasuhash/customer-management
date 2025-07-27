using Models.Infrastructure;
using Models.Infrastructure.Events;
using Models.Workflows.Events;

namespace Models.Workflows
{
    internal class LegalEntityEvaluationWorkflow : IWorkflow
    {
        public static int InstanceCount = 0;

        public void Run(IEventInfo eventInfo)
        {
            var legalEntityEvent = (LegalEntityChanged)eventInfo;

            EventAggregator.Log("LegalEntityEvaluationWorkflow Instance Count: {0}", ++InstanceCount);

            EventAggregator.Log($"<magenta> START: LegalEntityEvaluationWorkflow - LegalEntity Id:'{legalEntityEvent.Document.Id}', CloneInfo:'{legalEntityEvent.Document}'");

            var workingLegalEntity = legalEntityEvent.Document.Submitted;

            EventAggregator.Log("Processing LegalEntity Id:'{0}' with Name:'{1}', Legal name:'{2}'", legalEntityEvent.Document.Id, workingLegalEntity.Name, workingLegalEntity.LegalName); Thread.Sleep(3 * 1000);

            var customerFromDatabase = Database.Instance.CustomerDocuments.First(c => c.Id.Equals(workingLegalEntity.CustomerId));

            // There is no approved Customer yet. 
            if (customerFromDatabase.Approved == null)
            {
                EventAggregator.Log("Customer with Id:'{0}' not approved yet, but a change may be ongoing.", workingLegalEntity.CustomerId);

                var requireDependency = new EvaluationRequireDependency(legalEntityEvent.Document.Id, EntityName.LegalEntity, customerFromDatabase.Id, EntityName.Customer);

                EventAggregator.Publish(requireDependency);

                EventAggregator.Log($"<magenta> END: LegalEntityEvaluationWorkflow - LegalEntity Id:'{legalEntityEvent.Document.Id}'");

                return;
            }

            // At this point, we have a Customer that is Approved.
            EventAggregator.Log($"LegalEntity Id:'{legalEntityEvent.Document.Id}' Evaluation - Start");
            EventAggregator.Log($"--> Legal Entity System updated - {workingLegalEntity.LegalName} with Customer email '{((Customer)customerFromDatabase.Approved).EmailAddress}' <--");
            Thread.Sleep(5 * 1000);
            EventAggregator.Log($"LegalEntity Id:'{legalEntityEvent.Document.Id}' Evaluation - End");

            // Notify Orchestrator.
            Orchestrator.Instance.OnNotify(Result.EvaluationSuccessAndComplete(legalEntityEvent.Document.Id, legalEntityEvent.Document.SubmittedVersion, (legalEntityEvent.Document.Approved).LegalName));

            EventAggregator.Log($"<magenta> END: LegalEntityEvaluationWorkflow - LegalEntity Id:'{legalEntityEvent.Document.Id}'");
        }
    }
}
