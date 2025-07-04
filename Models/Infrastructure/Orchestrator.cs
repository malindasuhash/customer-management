using Models.Infrastructure.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public class Orchestrator
    {
        private readonly EntityManager _entityManager = new();

        public void QueueChangeForProcessing(IEventInfo eventInfo)
        {

        }

        public void OnCustomerChanged(IEventInfo eventInfo)
        {
            var customerChanged = (CustomerChanged)eventInfo;

            // TODO: I may need to change it to get the lastest submitted entities
            var latestCustomerChange = Database.Instance.GetLatestSubmittedCustomer(customerChanged.CustomerId);
            _entityManager.Transition(latestCustomerChange);
            Database.Instance.MarkAsWorkingCopy(latestCustomerChange);

            // TODO: Trigger workflow
            EventAggregator.Publish(new CustomerWorkflowEvent(latestCustomerChange.Id, latestCustomerChange.SubmittedVersion));

            /* Steps:
             * 1. Lock customer - later
             * 2. Move latest submitted data to evaluation
             * 3. If latest being used then no need to create further copies
             * 4. Trigger new workflow
             */
        }
    }
}
