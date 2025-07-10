using Models.Infrastructure;
using Models.Infrastructure.Events;
using Models.Workflows.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Workflows
{
    internal class CustomerPostApplyWorkflow : IWorkflow
    {
        public void Run(IEventInfo eventInfo)
        {
            // This workflow is triggered after the Customer has been applied to the database
            var customerEvent = (CustomerSynchonised)eventInfo;

            EventAggregator.Log("<magenta> START: CustomerPostApplyWorkflow - Customer Id:'{0}', Version:{1}", customerEvent.CustomerId, customerEvent.Version);

            // Trigger Legal Entity Evaluation Workflow?
            // Get Legal entity for this Customer

            // Check whether there are any legal entities for this customer
            var anyLegalEntities = Database.Instance.LegalEntityCollection.Count() == 0;

            if (anyLegalEntities)
            {
                EventAggregator.Log("<magenta> END: CustomerPostApplyWorkflow - No linked legal entities - Customer Id:'{0}', Version:{1}", customerEvent.CustomerId, customerEvent.Version);
                return;
            }

            // Get list of legal entities for this customer
            var legalEntities = Database.Instance.LegalEntityCollection
                .Where(legalEntity => legalEntity.ClientCopy.CustomerId.Equals(customerEvent.CustomerId))
                .ToList();

            foreach (var legalEntity in legalEntities)
            {
                Task.Run(() =>
                {
                    Orchestrator.Instance.Touch(
                        Result.Evaluate(legalEntity.Id, EntityName.LegalEntity),
                        Result.EvaluationContext(customerEvent.CustomerId, customerEvent.Version, EntityName.Customer));
                });
            }

            EventAggregator.Log("<magenta> END: CustomerPostApplyWorkflow - Customer Id:'{0}', Version:{1}", customerEvent.CustomerId, customerEvent.Version);
        }
    }
}
