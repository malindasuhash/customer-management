using Models.Infrastructure;
using Models.Infrastructure.Events;
using Models.Workflows.Events;

namespace Models.Workflows
{
    internal class CustomerApplyWorkflow : IWorkflow
    {
        public void Run(IEventInfo eventInfo)
        {
            var customerEvent = (CustomerEvaluationSuccessEvent)eventInfo;

            EventAggregator.Log("<magenta> START: CustomerApplyWorkflow - Customer Id:'{0}', Submitted Version:{1}", customerEvent.CustomerId, customerEvent.Document.SubmittedVersion);

            EventAggregator.Log("CustomerApplyWorkflow - Applying change to Identity & Authorisation system for Customer:'{0}'", customerEvent.CustomerId); Thread.Sleep(3 * 1000);

            EventAggregator.Publish(customerEvent.Document.Applied());

            EventAggregator.Log("<magenta> END: CustomerApplyWorkflow - Customer Id:'{0}', Submitted Version:{1}", customerEvent.CustomerId, customerEvent.Document.SubmittedVersion);
        }
    }
}
