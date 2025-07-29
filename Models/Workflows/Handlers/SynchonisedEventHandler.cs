using Models.Infrastructure;
using Models.Infrastructure.Events;
using Models.Workflows.Events;

namespace Models.Workflows.Handlers
{
    public class SynchonisedEventHandler
    {
        public void Handle(IEventInfo synchonisedEvent)
        {
            var eventInfo = (CustomerSynchonisedEvent)synchonisedEvent;

            var customerDocument = Database.Instance.CustomerDocuments.First(c => c.Id == eventInfo.Document.Id);
            if (customerDocument.SubmittedVersion == eventInfo.Document.SubmittedVersion)
            {
                customerDocument.Approved = eventInfo.Document.Submitted.CloneCustomer();
                customerDocument.ApprovedVersion = eventInfo.Document.SubmittedVersion;
                Database.Instance.UpsertDocument(customerDocument);
                EventAggregator.Log($"Document 'Approved' is updated '{customerDocument.Id}'");
            }
            else
            {
                EventAggregator.Log($"<red> ERROR: SynchonisedEventHandler - Customer Id:'{eventInfo.CustomerId}' has a different version. Expected: {eventInfo.Document.SubmittedVersion}, Actual: {customerDocument.SubmittedVersion}");
                return; // Exit if the versions do not match
            }
        }
    }
}
