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
        internal void Change(Customer customer, bool submit)
        {
            // If customer is new then assign an identifier (CustomerId).
            Intialise(customer);

            // Client is submitting
            if (submit)
            {
                // This is a deep clone
                var entity = (Customer)customer.Clone();

                // Latest draft is marked for submission
                entity.SubmittedVersion = entity.DraftVersion; 

                // LastSubmittedVersion in the Draft is set
                customer.LastSubmittedVersion = entity.SubmittedVersion;
            }
        }

        private static void Intialise(Customer customer)
        {
            if (customer.Id is not null) return;

            customer.Id = Guid.NewGuid().ToString();
            customer.State = EntityState.Draft;
            customer.DraftVersion.Increment();
        }
    }
}
