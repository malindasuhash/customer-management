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

            EventAggregator.Log($"<magenta> START: CustomerPostApplyWorkflow - Customer Id:'{customerEvent.CustomerId}', Version:{customerEvent.Document.SubmittedVersion}");

            EventAggregator.Publish(customerEvent.Document.Synchonised());

            EventAggregator.Log($"<magenta> END: CustomerPostApplyWorkflow - Customer Id:'{customerEvent.CustomerId}', Version:{customerEvent.Document.SubmittedVersion}");
        }
    }
}
