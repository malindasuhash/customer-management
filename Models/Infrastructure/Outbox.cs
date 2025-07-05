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
        public void AsClientCopy(IClientEntity clientEntity)
        {
            Database.Instance.AddToClientCopy(clientEntity);
        }

        public void AsSubmittedCopy(ISubmittedEntity submittedEntity)
        {
            Database.Instance.AddToSubmittedCopy(submittedEntity);
        }

        internal void ReadyForEvalution(Customer latestCustomerChange)
        {
            Database.Instance.MarkAsWorkingCopy(latestCustomerChange);

            // Trigger event
            EventAggregator.Publish(latestCustomerChange.GetChangedEvent());
        }

        internal void ReadyToApply(Customer workingCopy)
        {
            // Update database

            // Trigger event
            EventAggregator.Publish(new CustomerEvaluationCompleteEvent(workingCopy.Id, workingCopy.SubmittedVersion, true));
        }
    }
}
