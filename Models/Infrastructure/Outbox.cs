using Models.Contract;
using Models.Infrastructure.Events;
using Models.Workflows;
using Models.Workflows.Events;
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
        public void NewClientCopy(IClientEntity clientEntity)
        {
            Database.Instance.AddToClientCopy(clientEntity);
        }

        public void UpdateClientCopy(IClientEntity clientEntity)
        {
            Database.Instance.UpdateClientCopy(clientEntity);
        }

        public void SubmittedCopy(ISubmittedEntity submittedEntity)
        {
            Database.Instance.AddToSubmittedCopy(submittedEntity);
        }

        internal void Evaluate(ISubmittedEntity latestEntityChange)
        {
            Database.Instance.MarkAsWorkingCopy(latestEntityChange);

            // Trigger event
            EventAggregator.Publish(latestEntityChange.GetChangedEvent());
        }

        internal void Apply(Customer workingCopy)
        {
            // Update state

            // Trigger event
            EventAggregator.Publish(new CustomerEvaluationCompleteEvent(workingCopy.Id, workingCopy.SubmittedVersion, true));
        }

        internal void Ready(Customer workingCopy)
        {
            var readyUpdateResult = Database.Instance.MarkAsReady(workingCopy);

            EventAggregator.Publish(new CustomerSynchonised(workingCopy.Id, workingCopy.SubmittedVersion));
        }

        internal void WorkingCopyfailed(Customer workingCopy)
        {
            Database.Instance.MarkAsReady(workingCopy);

            EventAggregator.Publish(new CustomerEvalidationFailed(workingCopy.Id, workingCopy.SubmittedVersion));
        }

        internal void DiscardWorkingCopy(Customer workingCopy)
        {
            Database.Instance.DiscardWorkingCopy(workingCopy);
        }
    }
}
