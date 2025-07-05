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

        public void AsSubmittedCopy(ISubmittedEntity submittedEntity)
        {
            Database.Instance.AddToSubmittedCopy(submittedEntity);
        }

        public void EntitySubmitted(ISubmittedEntity submittedEntity)
        {
            // Write to database
            Database.Instance.AddToSubmittedCopy(submittedEntity);

            // Notify Orchestrator
            Orchestrator.Instance.OnEntityUpdated(submittedEntity.GetChangedEvent());
        }
    }
}
