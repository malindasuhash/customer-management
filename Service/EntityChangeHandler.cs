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

            // Store in database
            _outbox.AsClientCopy(clientEntity);

            // Submit for processing 
            if (submit)
            {
                EventAggregator.Log("Processing submission, please wait....(simulating copy)"); // Thread.Sleep(2000);

                // TODO: I think I need to deep clone entire object hirarchy 
                var draftEntities = Database.Instance.GetDraftEntitiesFor(clientEntity.Id); // Customer is special. 

                foreach (var draftEntity in draftEntities)
                {
                    // Take a deep copy of latest "draft" version.
                    var entitytoSubmit = (ISubmittedEntity)draftEntity.Clone();
                    _entityManager.Transition(entitytoSubmit);

                    // In this case, entity is copied to submitted but change event is not raised.
                    if (draftEntity.DraftVersion == draftEntity.LastSubmittedVersion)
                    {
                        EventAggregator.Log("No change detected for Entity:'{0}' with Id:'{1}'", "EntityName", draftEntity.Id);

                        // But the entity needs to be copied to latest submitted state; but no event is raised.
                        _outbox.AsSubmittedCopy(entitytoSubmit);
                        return;
                    }

                    // Latest draft is marked for submission
                    draftEntity.LastSubmittedVersion = draftEntity.DraftVersion;

                    entitytoSubmit.SubmittedVersion = draftEntity.LastSubmittedVersion;
                    
                    // Perhaps an update to reflect new LastSubmittedVersion in client copy in case of a 
                    // proper database implementation.

                    // Copies to submitted and raises the change event
                    _outbox.AsSubmittedCopy(entitytoSubmit);

                    Orchestrator.Instance.EntitySubmitted(entitytoSubmit);

                    EventAggregator.Log("Entity cloned & submitted, \n Draft: [{0}], \n Submitted: [{1}]", draftEntity, entitytoSubmit);
                }
            }
        }

        private void Update(IClientEntity clientEntity)
        {
            if (clientEntity.Id is not null)
            {
                // Incrementing the version as there has been a change
                clientEntity.DraftVersion++;

                EventAggregator.Log("Entity Id {0} updated, State:'{1}' new Draft version:'{2}'",
                    clientEntity.Id,
                    clientEntity.State,
                    clientEntity.DraftVersion);
            }
            else
            {
                clientEntity.Id = Guid.NewGuid().ToString();
                clientEntity.DraftVersion = 1;
                _entityManager.Transition(clientEntity);
            }
        }
    }
}
