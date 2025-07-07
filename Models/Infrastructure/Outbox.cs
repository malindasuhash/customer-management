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
            var readyUpdateResult = Database.Instance.MarkAsReady(workingCopy);

            if (!readyUpdateResult)
            {
                // TODO: What should happen here? Do I rollback or re-trigger a new workflow?
                EventAggregator.Log("<red>STOP: Working copy is outdated. Higher version.");
                return;
            }

            EventAggregator.Publish(new CustomerSynchonised(workingCopy.Id, workingCopy.SubmittedVersion));
        }

        internal void WorkingCopyfailed(Customer workingCopy)
        {
            Database.Instance.MarkAsReady(workingCopy);

            EventAggregator.Publish(new CustomerEvalidationFailed(workingCopy.Id, workingCopy.SubmittedVersion));
        }
    }
}
