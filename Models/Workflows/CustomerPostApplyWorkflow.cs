using Models.Infrastructure;
using Models.Infrastructure.Events;
using Models.Workflows.Events;

namespace Models.Workflows
{
    internal class CustomerPostApplyWorkflow : IWorkflow
    {
        public void Run(IEventInfo eventInfo)
        {
            // This workflow is triggered after the Customer has been applied to the database
            var customerEvent = (CustomerAppliedEvent)eventInfo;

            EventAggregator.Log($"<magenta> START: CustomerPostApplyWorkflow - Customer Id:'{customerEvent.CustomerId}', Version:{customerEvent.Document.SubmittedVersion}");

            EventAggregator.Log($"Executing - Post-apply processing for Customer:'{customerEvent.CustomerId}'");
            Thread.Sleep(3 * 1000); // Simulate some processing time

            EventAggregator.Publish(customerEvent.Document.Synchonised());

            EventAggregator.Log($"<magenta> END: CustomerPostApplyWorkflow - Customer Id:'{customerEvent.CustomerId}', Version:{customerEvent.Document.SubmittedVersion}");
        }
    }
}
