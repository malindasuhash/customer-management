using Models.Contract;
using Models.Infrastructure;
using Models.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class EntityChangeHandler
    {
        private readonly Outbox _outbox = new();

        public void Manage<T>(IDocument<T> document) where T : class, IEntity, ICloneable, new()
        {
            if (document.Id is not null)
            {
                // Incrementing the version as there has been a change
                document.DraftVersion++;

                EventAggregator.Log($"Entity Id {document.Id} updated, State:'{document.CurrentState}' new Draft version:'{document.DraftVersion}'");

                // Update in database
                _outbox.UpdateOrInsert(document);
            }
            else
            {
                DocumentStateManager.Instance.Transition(document);

                document.Id = Guid.NewGuid().ToString();
                document.DraftVersion = 1;

                // New in database
                _outbox.UpdateOrInsert(document);
            }
        }

        public void Submit(string entityId, string entityName)
        {
            EventAggregator.Log("Identifying changed entities to process.");

            var impactedCustomer = Database.Instance.CustomerDocuments
                .First(customer => customer.Id.Equals(entityId));

            // Check if the entity has been changed since last submission
            if (impactedCustomer.DraftVersion != impactedCustomer.SubmittedVersion)
            {
                EventAggregator.Log($"Customer {impactedCustomer.Id} has been changed and is ready for submission.");
                DocumentStateManager.Instance.Transition(impactedCustomer);
            }

            // Find out all linked entities related to Customer
            var linkedEntities = Database.Instance.LegalEntityDocuments
                .Where(entity => entity.Draft.CustomerId.Equals(entityId))
                .ToList();

            foreach (var entity in linkedEntities)
            {
                if (entity.DraftVersion != entity.SubmittedVersion)
                {
                    DocumentStateManager.Instance.Transition(entity);

                    // Raise changed event
                    EventAggregator.Log($"Legal Entity {entity.Id} has been changed and is ready for submission.");
                }
                else
                {
                    EventAggregator.Log($"Legal Entity {entity.Id} has not been changed since last submission.");
                }
            }
        }

        public void MoveFromSubmittedToWorkingCopy(string entityId, string entityName, int version)
        {
            var latestSubmitted = Database.Instance.GetLatestSubmittedEntity(entityId, entityName);

            // Move latest submitted entity to working copy state
            // _stateManager.Transition(latestSubmitted);
            _outbox.MoveFromSubmittedToWorking(latestSubmitted);
        }
    }
}
