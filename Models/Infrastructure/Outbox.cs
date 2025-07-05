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
        public void AsClientCopy(IClientEntity clientEntity)
        {
            Database.Instance.AddToClientCopy(clientEntity);
        }

        public void AsSubmittedCopy(IClientEntity draftEntity)
        {
            throw new NotImplementedException();
        }

        public void EntityChanged(ISubmittedEntity submittedEntity)
        {
            // Write to database
            Database.Instance.AddToSubmittedCopy(submittedEntity);

            // Publish event
            EventAggregator.Publish(new CustomerChanged(submittedEntity.Id, submittedEntity.SubmittedVersion));

         
        }

        public void Update(IClientEntity clientEntity)
        {
            throw new NotImplementedException();
        }
    }
}
