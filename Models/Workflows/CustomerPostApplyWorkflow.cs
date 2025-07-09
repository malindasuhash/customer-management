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
            Database.Instance.LegalEntityCollection
                .Where(legalEntity => legalEntity.ClientCopy.CustomerId.Equals(customerEvent.CustomerId))
                .ToList()
                .ForEach(legalEntity =>
                {
                   Orchestrator.Instance.Touch(
                       Result.Evaluate(legalEntity.Id, EntityName.LegalEntity), 
                       Result.EvaluationFailed(customerEvent.CustomerId, customerEvent.Version, EntityName.Customer));
                });

            EventAggregator.Log("<magenta> END: CustomerPostApplyWorkflow - Customer Id:'{0}', Version:{1}", customerEvent.CustomerId, customerEvent.Version);


        }
    }
}
