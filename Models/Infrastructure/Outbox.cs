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
        public void EntityChanged(ISubmittedEntity submittedEntity)
        {
            EventAggregator.Publish(new CustomerChanged(submittedEntity.Id, submittedEntity.SubmittedVersion));

            // Write to database
        }
    }
}
