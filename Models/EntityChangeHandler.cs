using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Infrastructure;
using Models.Infrastructure.Events;
using Models.Contract;

namespace Models
{
    public class EntityChangeHandler
    {
        private readonly Outbox _outbox = new();
        private readonly EntityManager _entityManager = new();

        public void Change(IClientEntity clientEntity, bool submit)
        {
            UpdateClientCopy(clientEntity);

            // Submit for processing 
            if (submit)
            {
                EventAggregator.Log("Processing submission");

                // TODO: I think I need to deep clone entire object hirarchy 
                var draftEntities = Database.Instance.GetDraftEntitiesFor(clientEntity.Id, clientEntity.Name);

                foreach (var draftEntity in draftEntities)
                {
                    // Take a deep copy of latest "draft" version.
                    var entitytoSubmit = (ISubmittedEntity)draftEntity.Clone();
                    _entityManager.Transition(entitytoSubmit);

                    // Latest draft is marked for submission
                    draftEntity.LastSubmittedVersion = draftEntity.DraftVersion;
                    entitytoSubmit.SubmittedVersion = draftEntity.LastSubmittedVersion;

                    // Perhaps an update to reflect new LastSubmittedVersion in client copy in case of a 
                    // proper database implementation.

                    // Copies to submitted and raises the change event
                    _outbox.SubmittedCopy(entitytoSubmit);

                    EventAggregator.Log("Entity cloned & submitting, \n Draft: [{0}], \n Submitted: [{1}]", draftEntity, entitytoSubmit);

                    // Notify the event aggregator that the entity has been submitted, But only if the Ids match.
                    if (clientEntity.Id != draftEntity.Id) return;

                    EventAggregator.Publish(new EntitySubmitted(entitytoSubmit.Id, entitytoSubmit.Name, entitytoSubmit.SubmittedVersion));
                }
            }
        }

        private void UpdateClientCopy(IClientEntity clientEntity)
        {
            if (clientEntity.Id is not null)
            {
                // Incrementing the version as there has been a change
                clientEntity.DraftVersion++;

                EventAggregator.Log("Entity Id {0} updated, State:'{1}' new Draft version:'{2}'",
                    clientEntity.Id,
                    clientEntity.State,
                    clientEntity.DraftVersion);

                // Update in database
                _outbox.UpdateClientCopy(clientEntity);
            }
            else
            {
                clientEntity.Id = Guid.NewGuid().ToString();
                clientEntity.DraftVersion = 1;
                _entityManager.Transition(clientEntity);

                // New in database
                _outbox.NewClientCopy(clientEntity);
            }
        }
    }
}
