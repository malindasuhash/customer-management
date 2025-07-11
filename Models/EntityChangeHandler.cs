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

        public void Manage(IClientEntity clientEntity, bool submit)
        {
            CreateOrUpdate(clientEntity);

            if (!submit) return;

            EventAggregator.Log("Copy latest draft to submitted state.");
            var draftEntities = Database.Instance.GetLatestDraft(clientEntity.Id, clientEntity.Name);

            // Copy the draft entity to submitted state
            foreach (var draftEntity in draftEntities)
            {
                // Take a deep copy of latest "draft" version.
                var entityToSubmit = (ISubmittedEntity)draftEntity.Clone();
                _entityManager.Transition(entityToSubmit);
                EventAggregator.Log("Entity cloned & submitting, \n Draft: [{0}], \n Submitted: [{1}]", draftEntity, entityToSubmit);

                // Latest draft is marked for submission
                draftEntity.LastSubmittedVersion = draftEntity.DraftVersion;
                entityToSubmit.SubmittedVersion = draftEntity.LastSubmittedVersion;

                // Copies to submitted and raises the change event
                _outbox.Submit(entityToSubmit);
            }
        }

        public void MoveFromSubmittedToWorkingCopy(string entityId, string entityName, int version)
        {
            var latestSubmitted = Database.Instance.GetLatestSubmittedEntity(entityId, entityName);

            // Move latest submitted entity to working copy state
            _entityManager.Transition(latestSubmitted);
            _outbox.MoveFromSubmittedToWorking(latestSubmitted);
        }

        private void CreateOrUpdate(IClientEntity clientEntity)
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
