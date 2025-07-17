using Models.Infrastructure;
using Models.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Models.Workflows
{
    internal class LegalEntityEvaluationWorkflow : IWorkflow
    {
        public static int InstanceCount = 0;

        public void Run(IEventInfo eventInfo)
        {
            var legalEntityEvent = (LegalEntityChanged)eventInfo;

            EventAggregator.Log("LegalEntityEvaluationWorkflow Instance Count: {0}", ++InstanceCount);

            EventAggregator.Log("<magenta> START: LegalEntityEvaluationWorkflow - LegalEntity Id:'{0}', Version:{1}", legalEntityEvent.LegalEntityId, legalEntityEvent.Version);

            var workingLegalEntity = Database.Instance.LegalEntityCollection.First(entry => entry.Id.Equals(legalEntityEvent.LegalEntityId)).WorkingCopy.First(ver => ver.SubmittedVersion == eventInfo.Version);

            EventAggregator.Log("Processing LegalEntity Id:'{0}' with Name:'{1}', Legal name:'{2}'", workingLegalEntity.Id, workingLegalEntity.Name, workingLegalEntity.LegalName); Thread.Sleep(3 * 1000);

            var customer = Database.Instance.CustomerCollection.First(c => c.Id.Equals(workingLegalEntity.CustomerId));

            if (customer.WorkingCopy != null && customer.WorkingCopy.Count() > 0) // Customer is in progress
            {
                EventAggregator.Log("Customer Id:'{0}' is in progress, waiting for it to complete.", customer.Id);
                
                EventAggregator.Log("<magenta> END: LegalEntityEvaluationWorkflow - LegalEntity Id:'{0}', Version:{1}", legalEntityEvent.LegalEntityId, legalEntityEvent.Version);
                return;
            }

            if (customer.ReadyCopy == null) // Customer is not ready
            {
                EventAggregator.Log("Awaiting for the Customer dependency to resolve");

                EventAggregator.Log("<magenta> END: LegalEntityEvaluationWorkflow - LegalEntity Id:'{0}', Version:{1}", legalEntityEvent.LegalEntityId, legalEntityEvent.Version);

                Orchestrator.Instance.OnNotify(Result.EvaluationWaitingDependency(customer.Id, 0, EntityName.Customer));
                return;
            }
            
            EventAggregator.Log("LegalEntity Id:'{0}' Evaluation - Start", workingLegalEntity.Id);
            EventAggregator.Log($"--> Legal Entity System updated - {workingLegalEntity.LegalName} with Customer email '{customer.ReadyCopy.EmailAddress}' <--");
            Thread.Sleep(5 * 1000);
            EventAggregator.Log("LegalEntity Id:'{0}' Evaluation - End", workingLegalEntity.Id);

            ReEvaluateCustomerIfNeeded(customer, workingLegalEntity, legalEntityEvent);

            // Notify Orchestrator.
            Orchestrator.Instance.OnNotify(Result.EvaluationSuccessAndComplete(workingLegalEntity.Id, workingLegalEntity.SubmittedVersion, workingLegalEntity.Name));

            EventAggregator.Log("<magenta> END: LegalEntityEvaluationWorkflow - LegalEntity Id:'{0}', Version:{1}", legalEntityEvent.LegalEntityId, legalEntityEvent.Version);
        }

        private void ReEvaluateCustomerIfNeeded(EntityLayout<Customer, CustomerClient> customer, LegalEntity legalEntity, LegalEntityChanged legalEntityEvent)
        {
            // Imagine that in some cases, the Customer needs to be re-evaluated.
            if (legalEntity.LegalName.StartsWith("W") 
                && customer.ReadyCopy != null  // There is a Ready copy
                && customer.LastestSubmittedCopy == null // There is nothing submitted for processing
                && customer.WorkingCopy.Count() == 0) // There is nothing in progress
            {
                // Notify Orchestrator that the Customer needs to be re-evaluated.
                //Task.Run(() =>
                //{
                //    Orchestrator.Instance.OnNotify(Result.ReEvaluate(customer.Id, EntityName.Customer));
                //});
            }
        }
    }
}
