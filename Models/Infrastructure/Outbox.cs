using Models.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public class Outbox
    {
        public void CustomerChanged(Customer customer)
        {
            EventAggregator.Publish(new CustomerChanged(customer.Id, customer.SubmittedVersion));

            // Write to database
        }
    }
}
