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
        public static Orchestrator Instance = new();

        private Orchestrator(){ }

        private readonly EntityManager _entityManager = new();

        public void OnEntityUpdated(IEventInfo eventInfo)
        {
            var customerChanged = (CustomerChanged)eventInfo;

            // TODO: I may need to change it to get the lastest submitted entities
            var latestCustomerChange = Database.Instance.GetLatestSubmittedCustomer(customerChanged.CustomerId);
            _entityManager.Transition(latestCustomerChange);
            Database.Instance.MarkAsWorkingCopy(latestCustomerChange);

            // Trigger workflow
            EventAggregator.Publish(eventInfo);
        }
    }
}
