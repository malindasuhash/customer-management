using Models.Infrastructure;
using Models.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Workflows
{
    internal class LegalEntityEvaluationWorkflow : IWorkflow
    {
        public void Run(IEventInfo eventInfo)
        {
            var legalEntityEvent = (LegalEntityChanged)eventInfo;

            EventAggregator.Log("<magenta> START: LegalEntityEvaluationWorkflow - LegalEntity Id:'{0}', Version:{1}", legalEntityEvent.LegalEntityId, legalEntityEvent.Version);

            var workingLegalEntity = Database.Instance.LegalEntityCollection.First(entry => entry.Id.Equals(legalEntityEvent.LegalEntityId)).WorkingCopy.First(ver => ver.SubmittedVersion == eventInfo.Version);

            EventAggregator.Log("Processing LegalEntity Id:'{0}' with Name:'{1}', Legal name:'{2}'", workingLegalEntity.Id, workingLegalEntity.Name, workingLegalEntity.LegalName); Thread.Sleep(3 * 1000);

            // Is there is valid customer? if not then "touch" the Customer.
            var customer = Database.Instance.CustomerCollection.First(c => c.Id.Equals(workingLegalEntity.CustomerId));

            // If there are submitted copied then we need to evaluate the Customer.
            if (customer.ReadyCopy == null || customer.LastestSubmittedCopy != null)
            {
                // Notify Orchestrator that the Customer is not ready.
                EventAggregator.Log($"<red> [TOUCH] Customer '{workingLegalEntity.CustomerId}' is not ready or data sumitted; require evaluation.");

                Task.Run(() =>
                {
                    Orchestrator.Instance.Touch(
                        Result.Evaluate(workingLegalEntity.CustomerId, EntityName.Customer),
                        Result.EvaluationContext(legalEntityEvent.LegalEntityId, legalEntityEvent.Version, EntityName.LegalEntity));
                });
                
                return;
            }
            else
            {
                Orchestrator.Instance.OnNotify(Result.EvaluationSuccessAndComplete(legalEntityEvent.LegalEntityId, legalEntityEvent.Version, EntityName.LegalEntity));
            }

            EventAggregator.Log("<magenta> END: LegalEntityEvaluationWorkflow - LegalEntity Id:'{0}', Version:{1}", legalEntityEvent.LegalEntityId, legalEntityEvent.Version);
        }
    }
}
