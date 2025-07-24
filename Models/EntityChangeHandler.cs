using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Infrastructure;
using Models.Infrastructure.Events;
using Models.Contract;
using System.Security.Claims;
using System.Runtime.InteropServices;

namespace Models
{
    public class EntityChangeHandler
    {
        private readonly Outbox _outbox = new();
        private readonly DocumentStateManager _stateManager = new();

        public void Manage<T>(IDocument<T> document) where T : class, IEntity, new()
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
                _stateManager.Transition(document);

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
                impactedCustomer.Submitted = (Customer)impactedCustomer.Draft.Clone();
                impactedCustomer.SubmittedVersion = impactedCustomer.DraftVersion;

                var changedCustomerDocumentToSubmit = (CustomerDocument)impactedCustomer.Clone();

                // Raise changed event
            }

            // Find out all linked entities related to Customer


            //var draftEntities = Database.Instance.GetLatestDraft(entityId, entityName);

            // Copy the draft entity to submitted state
            //foreach (var draftEntity in draftEntities)
            //{

                //// Entity unchanged, therefore no need to submit
                //if (draftEntity.DraftVersion == draftEntity.DraftVersion) continue;

                //// Take a deep copy of latest "draft" version.
                //var entityToSubmit = (ISubmittedEntity)draftEntity.Clone();
                //_stateManager.Transition(entityToSubmit);
                //EventAggregator.Log("Entity cloned & submitting, \n Draft: [{0}], \n Submitted: [{1}]", draftEntity, entityToSubmit);

                //draftEntity.LastSubmittedVersion = draftEntity.DraftVersion;
                //entityToSubmit.SubmittedVersion = draftEntity.LastSubmittedVersion;
                //_outbox.Submit(entityToSubmit);
            //}
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
