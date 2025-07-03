using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Infrastructure;

namespace Service
{
    internal class EntityChangeHandler
    {
        internal void Change(CustomerClient customerClient, bool submit)
        {
            Update(customerClient);

            // Submit for processing 
            if (submit)
            {
                // Deep clone into an entity
                var entitytoSubmit = (Customer)customerClient.Clone();
                entitytoSubmit.State = EntityState.Submitted;

                // Latest draft is marked for submission
                // We'll take the latest draft from client
                customerClient.LastSubmittedVersion = customerClient.DraftVersion; 
                
                entitytoSubmit.SubmittedVersion = customerClient.LastSubmittedVersion;

                EventAggregator.Log("Entity Cloned, ready for submission \n Draft: [{0}], \n Submitted: [{1}]", customerClient, entitytoSubmit);
            }
        }

        private static void Update(CustomerClient customer)
        {
            if (customer.Id is not null) { 
                customer.DraftVersion++;

                EventAggregator.Log("Entity Id {0} updated, new Draft version: {1}", 
                    customer.Id, 
                    customer.DraftVersion);
            } else
            {
                customer.Id = Guid.NewGuid().ToString();
                customer.State = EntityState.Draft;
                customer.DraftVersion = 1;

                EventAggregator.Log("New Entity Id {0} is set", customer.Id);
            }
        }
    }
}
