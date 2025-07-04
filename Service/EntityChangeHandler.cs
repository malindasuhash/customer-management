using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Infrastructure;
using Models.Infrastructure.Events;

namespace Service
{
    internal class EntityChangeHandler
    {
        private readonly Outbox _outbox = new();
        private readonly EntityManager _entityManager = new();

        internal void Change(CustomerClient customerClient, bool submit)
        {
            Update(customerClient);

            // Submit for processing 
            if (submit)
            {
                EventAggregator.Log("Processing submission, please wait....(simulating copy)"); Thread.Sleep(4000);

                // Deep clone into an entity
                var entitytoSubmit = (Customer)customerClient.Clone();
                _entityManager.Transition(entitytoSubmit);

                // Latest draft is marked for submission
                // We'll take the latest draft from client
                customerClient.LastSubmittedVersion = customerClient.DraftVersion; 
                
                entitytoSubmit.SubmittedVersion = customerClient.LastSubmittedVersion;

                EventAggregator.Log("Entity Cloned, ready for submission \n Draft: [{0}], \n Submitted: [{1}]", customerClient, entitytoSubmit);

                _outbox.CustomerChanged(entitytoSubmit);
            }
        }

        private void Update(CustomerClient customer)
        {
            if (customer.Id is not null) { 
                customer.DraftVersion++;

                EventAggregator.Log("Entity Id {0} updated, State:'{1}' new Draft version:'{2}'", 
                    customer.Id, 
                    customer.State,
                    customer.DraftVersion);
            } else
            {
                customer.Id = Guid.NewGuid().ToString();
                customer.DraftVersion = 1;
                _entityManager.Transition(customer);

                EventAggregator.Log("New Entity Id {0} is set, State:'{1}'", customer.Id, customer.State);
            }
        }
    }
}
