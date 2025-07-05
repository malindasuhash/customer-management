using Models.Infrastructure.Events;
using Models.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public class Outbox
    {
        public void AsNewClientCopy(IClientEntity clientEntity)
        {
            Database.Instance.AddToClientCopy(clientEntity);
        }

        public void AsUpdateClientCopy(IClientEntity clientEntity)
        {
            Database.Instance.UpdateClientCopy(clientEntity);
        }

        public void AsSubmittedCopy(ISubmittedEntity submittedEntity)
        {
            Database.Instance.AddToSubmittedCopy(submittedEntity);
        }

        internal void Evaluate(Customer latestCustomerChange)
        {
            Database.Instance.MarkAsWorkingCopy(latestCustomerChange);

            // Trigger event
            EventAggregator.Publish(latestCustomerChange.GetChangedEvent());
        }

        internal void Apply(Customer workingCopy)
        {
            // Update state

            // Trigger event
            EventAggregator.Publish(new CustomerEvaluationCompleteEvent(workingCopy.Id, workingCopy.SubmittedVersion, true));
        }

        internal void Ready(Customer workingCopy)
        {
            Database.Instance.MarkAsReady(workingCopy);

            EventAggregator.Publish(new CustomerReady(workingCopy.Id, workingCopy.SubmittedVersion));
        }
    }
}
