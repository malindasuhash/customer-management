using Models.Infrastructure;
using Models.Infrastructure.Events;
using Models.Workflows.Events;

namespace Models.Workflows
{
    internal class CustomerEvaluationWorkflow : IWorkflow
    {
        public void Run(IEventInfo eventInfo)
        {
            var customerEvent = (CustomerChanged)eventInfo;

            EventAggregator.Log($"<magenta> START: CustomerEvaluationWorkflow - Customer Id:'{customerEvent.CustomerId}'");

            var submittedDocument = customerEvent.Document.Submitted;

            if (submittedDocument.EmailAddress.Contains("bad", StringComparison.InvariantCultureIgnoreCase))
            {
                EventAggregator.Log($"CustomerEvaluationWorkflow - bad email:'{submittedDocument.EmailAddress}' for Customer:'{customerEvent.CustomerId}'"); Thread.Sleep(1000);

                EventAggregator.Publish(new EvaluationFailedEvent(customerEvent.CustomerId, EntityName.Customer, $"Email '{submittedDocument.EmailAddress}' is invalid, please correct and resubmit."));

                EventAggregator.Log($"<magenta> END: CustomerEvaluationWorkflow - Customer Id:'{customerEvent.CustomerId}'");

                return;
            }

            // Perform evaluation.
            var secondsToWait = submittedDocument.EmailAddress.Contains("wait") ? int.Parse(submittedDocument.EmailAddress.Replace("wait", string.Empty)) : 1;
            EventAggregator.Log($"CustomerEvaluationWorkflow - valid email:'{submittedDocument.EmailAddress}' for Customer:'{customerEvent.CustomerId}'"); Thread.Sleep(secondsToWait * 1000);

            EventAggregator.Publish(new EvaluationCompleteEvent(customerEvent.CustomerId, EntityName.Customer));

            EventAggregator.Log($"<magenta> END: CustomerEvaluationWorkflow - Customer Id:'{customerEvent.CustomerId}', Version: {1}");
        }
    }
}
