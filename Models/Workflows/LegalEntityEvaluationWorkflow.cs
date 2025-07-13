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

            if (!CanProceed(customer, workingLegalEntity, legalEntityEvent))
            {
                EventAggregator.Log("Awaiting for a dependency to resolve", workingLegalEntity.Id, workingLegalEntity.Name, workingLegalEntity.LegalName); 
                Orchestrator.Instance.OnNotify(Result.EvaluationWaitingDependency(workingLegalEntity.Id, workingLegalEntity.SubmittedVersion, workingLegalEntity.Name));
                return;
            }
            
            EventAggregator.Log("LegalEntity Id:'{0}' Evaluation - Start", workingLegalEntity.Id);
            Thread.Sleep(5 * 1000);
            EventAggregator.Log("LegalEntity Id:'{0}' Evaluation - End", workingLegalEntity.Id);

            ReEvaluateCustomerIfNeeded(customer, workingLegalEntity, legalEntityEvent);

            // Notify Orchestrator.
            Orchestrator.Instance.OnNotify(Result.EvaluationSuccessAndComplete(workingLegalEntity.Id, workingLegalEntity.SubmittedVersion, workingLegalEntity.Name));

            EventAggregator.Log("<magenta> END: LegalEntityEvaluationWorkflow - LegalEntity Id:'{0}', Version:{1}", legalEntityEvent.LegalEntityId, legalEntityEvent.Version);
        }

        private bool CanProceed(EntityLayout<Customer, CustomerClient> customer, LegalEntity legalEntity, LegalEntityChanged legalEntityEvent)
        {
            // There is a customer currently in progress.
            if (customer.WorkingCopy != null && customer.WorkingCopy.Any())
            {
                EventAggregator.Log("<yellow> There is a working copy for Customer '{0}' - Therefore I need to stop.", legalEntity.CustomerId);
                EventAggregator.Log("<magenta> END: LegalEntityEvaluationWorkflow - LegalEntity Id:'{0}', Version:{1}", legalEntityEvent.LegalEntityId, legalEntityEvent.Version);
                return false;
            }

            // There is a customer scheduled change.
            if (customer.LastestSubmittedCopy != null)
            {
                EventAggregator.Log("<yellow> There is a LastestSubmittedCopy for Customer '{0}' - Therefore I need to stop.", legalEntity.CustomerId);
                EventAggregator.Log("<magenta> END: LegalEntityEvaluationWorkflow - LegalEntity Id:'{0}', Version:{1}", legalEntityEvent.LegalEntityId, legalEntityEvent.Version);
                return false;
            }

            return true;
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
