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

        internal void Change(IClientEntity clientEntity, bool submit)
        {
            Update(clientEntity);

            // Submit for processing 
            if (submit)
            {
                EventAggregator.Log("Processing submission, please wait....(simulating copy)"); Thread.Sleep(2000);

                // TODO: I think I need to deep clone entire object hirarchy 
                // and then find out what has changed. Thereafter raise the event.
                var entitytoSubmit = (ISubmittedEntity)clientEntity.Clone();
                _entityManager.Transition(entitytoSubmit);

                // Latest draft is marked for submission
                // We'll take the latest draft from client
                clientEntity.LastSubmittedVersion = clientEntity.DraftVersion; 
                
                entitytoSubmit.SubmittedVersion = clientEntity.LastSubmittedVersion;

                EventAggregator.Log("Entity Cloned, ready for submission \n Draft: [{0}], \n Submitted: [{1}]", clientEntity, entitytoSubmit);

                _outbox.EntityChanged(entitytoSubmit);
            }
            else
            {
                _outbox.Update(clientEntity);
            }
        }

        private void Update(IClientEntity clientEntity)
        {
            if (clientEntity.Id is not null) { 

                // Incrementing the version as there has been a change
                clientEntity.DraftVersion++;

                EventAggregator.Log("Entity Id {0} updated, State:'{1}' new Draft version:'{2}'", 
                    clientEntity.Id, 
                    clientEntity.State,
                    clientEntity.DraftVersion);
            } else
            {
                clientEntity.Id = Guid.NewGuid().ToString();
                clientEntity.DraftVersion = 1;
                _entityManager.Transition(clientEntity);

                EventAggregator.Log("New Entity Id {0} is set, State:'{1}'", clientEntity.Id, clientEntity.State);
            }
        }
    }
}
