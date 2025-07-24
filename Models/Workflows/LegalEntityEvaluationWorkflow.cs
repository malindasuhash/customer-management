using Models.Infrastructure;
using Models.Infrastructure.Events;

namespace Models.Workflows
{
    internal class LegalEntityEvaluationWorkflow : IWorkflow
    {
        public static int InstanceCount = 0;

        public void Run(IEventInfo eventInfo)
        {
            var legalEntityEvent = (LegalEntityChanged)eventInfo;

            EventAggregator.Log("LegalEntityEvaluationWorkflow Instance Count: {0}", ++InstanceCount);

            EventAggregator.Log($"<magenta> START: LegalEntityEvaluationWorkflow - LegalEntity Id:'{legalEntityEvent.LegalEntityDocument.Id}', CloneInfo:'{legalEntityEvent.LegalEntityDocument}'");

            var workingLegalEntity = ((LegalEntity)legalEntityEvent.LegalEntityDocument.Submitted);

            EventAggregator.Log("Processing LegalEntity Id:'{0}' with Name:'{1}', Legal name:'{2}'", legalEntityEvent.LegalEntityDocument.Id, workingLegalEntity.Name, workingLegalEntity.LegalName); Thread.Sleep(3 * 1000);

            var customerFromDatabase = Database.Instance.CustomerDocuments.First(c => c.Id.Equals(workingLegalEntity.CustomerId));

            // There is an ongoing change for the Customer, so we need to wait.
            if (customerFromDatabase.Approved == null && customerFromDatabase.Submitted != null)
            {
                EventAggregator.Log("Customer with Id:'{0}' not approved yet, but a change may be ongoing.", workingLegalEntity.CustomerId);

                Orchestrator.Instance.OnNotify(Result.EvaluationWaitingDependency(customerFromDatabase.Id, 0, EntityName.Customer));

                EventAggregator.Log($"<magenta> END: LegalEntityEvaluationWorkflow - LegalEntity Id:'{legalEntityEvent.LegalEntityDocument.Id}'");

                return;
            }

            // Ideally we should not hit this case, but just in case.
            if (customerFromDatabase.Approved == null) return;

            // At this point, we have a Customer that is Approved.
            EventAggregator.Log($"LegalEntity Id:'{legalEntityEvent.LegalEntityDocument.Id}' Evaluation - Start");
            EventAggregator.Log($"--> Legal Entity System updated - {workingLegalEntity.LegalName} with Customer email '{((Customer)customerFromDatabase.Approved).EmailAddress}' <--");
            Thread.Sleep(5 * 1000);
            EventAggregator.Log($"LegalEntity Id:'{legalEntityEvent.LegalEntityDocument.Id}' Evaluation - End");

            // Notify Orchestrator.
            Orchestrator.Instance.OnNotify(Result.EvaluationSuccessAndComplete(legalEntityEvent.LegalEntityDocument.Id, legalEntityEvent.LegalEntityDocument.SubmittedVersion, ((LegalEntity)legalEntityEvent.LegalEntityDocument.Approved).LegalName));

            EventAggregator.Log($"<magenta> END: LegalEntityEvaluationWorkflow - LegalEntity Id:'{legalEntityEvent.LegalEntityDocument.Id}'");
        }
    }
}
