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
        private Database Database { get; }

        private readonly EntityManager _entityManager = new();

        public Orchestrator(Database database) {
            Database = database;
        }

        public void QueueChangeForProcessing(IEventInfo eventInfo)
        {

        }

        public void OnCustomerChanged(IEventInfo eventInfo)
        {
            var customerChanged = (CustomerChanged)eventInfo;

            var latestCustomerChange = Database.GetLatestSubmittedCustomer(customerChanged.CustomerId);
            _entityManager.Transition(latestCustomerChange);

            /* Steps:
             * 1. Lock customer - later
             * 2. Move latest submitted data to evaluation
             * 3. If latest being used then no need to create further copies
             * 4. Trigger new workflow
             */
        }
    }
}
