using Models.Contract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Models.Infrastructure
{
    public class Database
    {
        public static Database Instance = new();

        public List<Document<Customer>> CustomerDocuments { get; private set; }
        public List<Document<LegalEntity>> LegalEntityDocuments { get; private set; } = new();

        static Database()
        {
            Instance.CustomerDocuments = new List<Document<Customer>>();
            Instance.LegalEntityDocuments = new List<Document<LegalEntity>>();
        }

        public void AddToClientCopy(IClientEntity clientEntity)
        {
            var name = clientEntity.Name.Replace("Client", string.Empty);

            switch (name)
            {
                case EntityName.Customer:
                    CustomerCollection.Add(new EntityLayout<Customer, CustomerClient> { Id = clientEntity.Id, ClientCopy = (CustomerClient)clientEntity });
                    break;

                case EntityName.LegalEntity:
                    LegalEntityCollection.Add(new EntityLayout<LegalEntity, LegalEntityClient> { Id = clientEntity.Id, ClientCopy = (LegalEntityClient)clientEntity });
                    break;
            }

            EventAggregator.Log("Client Entity:'{0}' with Id:'{1}' Added", name, clientEntity.Id);
        }

        public void UpdateClientCopy(IClientEntity clientEntity)
        {
            var name = clientEntity.Name.Replace("Client", string.Empty);

            switch (name)
            {
                case EntityName.Customer:
                    var layout = CustomerCollection.First(customer => customer.Id.Equals(clientEntity.Id));
                    layout.ClientCopy = (CustomerClient)clientEntity;
                    break;

                case EntityName.LegalEntity:
                    var legalEntityLayout = LegalEntityCollection.First(legalEntity => legalEntity.Id.Equals(clientEntity.Id));
                    legalEntityLayout.ClientCopy = (LegalEntityClient)clientEntity;
                    break;
            }

            EventAggregator.Log("Client Entity:'{0}' with Id:'{1}' Updated", name, clientEntity.Id);
        }

        public List<IClientEntity> GetDraftEntitiesFor(string entityId, string entityName)
        {
            // TODO: Build up entire object hirarchy
            // For now, just return the latest draft version of Customer and LegalEntity
            List<IClientEntity> draftEntities = new();

            // Customer is special, for now when it is changed, then I will process just that entity.
            if (EntityName.Customer.Equals(entityName, StringComparison.OrdinalIgnoreCase))
            {
                draftEntities.Add(CustomerCollection
                   .Where(client => client.Id == entityId && client.ClientCopy.State.Equals(EntityState.Draft))
                   .Select(a => (IClientEntity)a.ClientCopy)
                   .First());
            }

            // When the change is related to LegalEntity then I can find both CustomerId and LegalEntityId.
            if (EntityName.LegalEntity.Equals(entityName, StringComparison.OrdinalIgnoreCase))
            {
                var legalEntity = (LegalEntityClient)LegalEntityCollection
                    .Where(client => client.Id == entityId && client.ClientCopy.State.Equals(EntityState.Draft))
                    .Select(a => (IClientEntity)a.ClientCopy)
                    .First();
                draftEntities.Add(legalEntity);

                draftEntities.Add(CustomerCollection
                    .Where(client => client.Id == legalEntity.CustomerId && client.ClientCopy.State.Equals(EntityState.Draft))
                    .Select(a => (IClientEntity)a.ClientCopy)
                    .First());
            }

            return draftEntities;
        }

        public IList<IClientEntity> GetLatestDraft(string entityId, string entityName)
        {
            var latestDraft = new List<IClientEntity>();
            IClientEntity? customer = null;

            if (EntityName.LegalEntity.Equals(entityName, StringComparison.OrdinalIgnoreCase))
            {
                var legalEntity = LegalEntityCollection
                    .Where(client => client.Id == entityId && client.ClientCopy.State.Equals(EntityState.Draft))
                    .Select(a => (IClientEntity)a.ClientCopy)
                    .FirstOrDefault();

                latestDraft.Add(legalEntity);

                customer = CustomerCollection
                    .Where(client => client.Id == ((LegalEntityClient)legalEntity).CustomerId && client.ClientCopy.State.Equals(EntityState.Draft))
                    .Select(a => (IClientEntity)a.ClientCopy)
                    .FirstOrDefault();

                latestDraft.Add(customer);
            }

            if (EntityName.Customer.Equals(entityName, StringComparison.OrdinalIgnoreCase))
            {
                customer = CustomerCollection
                      .Where(client => client.Id == entityId && client.ClientCopy.State.Equals(EntityState.Draft))
                      .Select(a => (IClientEntity)a.ClientCopy)
                      .FirstOrDefault();

                latestDraft.Add(customer);

                var legalEntity = LegalEntityCollection
                   .Where(client => client.ClientCopy.CustomerId.Equals(customer.Id) && client.ClientCopy.State.Equals(EntityState.Draft))
                   .Select(a => (IClientEntity)a.ClientCopy);

                if (legalEntity != null && legalEntity.Any())
                {
                    foreach (var le in legalEntity)
                    {
                        latestDraft.Add(le);
                    }
                }
            }

            return latestDraft;
        }

        internal void AddToSubmittedCopy(ISubmittedEntity entitytoSubmit)
        {
            switch (entitytoSubmit.Name)
            {
                case EntityName.Customer:
                    var layout = CustomerCollection.First(customer => customer.Id.Equals(entitytoSubmit.Id));
                    layout.SetLatestSubmittedCopy(entitytoSubmit);
                    break;

                case EntityName.LegalEntity:
                    var legalEntityLayout = LegalEntityCollection.First(legalEntity => legalEntity.Id.Equals(entitytoSubmit.Id));
                    legalEntityLayout.SetLatestSubmittedCopy(entitytoSubmit);
                    break;
            }

            EventAggregator.Log("Submitted Entity:'{0}' with Id:'{1}' Added", entitytoSubmit.Name, entitytoSubmit.Id);
        }

        internal ISubmittedEntity? GetLatestSubmittedEntity(string entityId, string entityName)
        {
            ISubmittedEntity returnThis = null;
            switch (entityName)
            {
                case EntityName.Customer:
                    returnThis = CustomerCollection.FirstOrDefault(customer => customer.Id.Equals(entityId, StringComparison.Ordinal))?.LastestSubmittedCopy;
                    break;

                case EntityName.LegalEntity:
                    returnThis = LegalEntityCollection.FirstOrDefault(legalEntity => legalEntity.Id.Equals(entityId, StringComparison.Ordinal))?.LastestSubmittedCopy;
                    break;
            }

            if (returnThis == null)
            {
                var aa = "d";
            }

            return returnThis;
        }

        internal ISubmittedEntity? GetWorkingCopy(string id, int version, string entityName)
        {
            switch (entityName)
            {
                case EntityName.Customer:
                    return CustomerCollection.FirstOrDefault(customer => customer.Id.Equals(id, StringComparison.Ordinal))?.WorkingCopy?.FirstOrDefault();

                case EntityName.LegalEntity:
                    return LegalEntityCollection.FirstOrDefault(legalEntity => legalEntity.Id.Equals(id, StringComparison.Ordinal))?.WorkingCopy?.FirstOrDefault();
            }

            return null;
        }

        internal void MarkAsWorkingCopy(ISubmittedEntity latestChange)
        {
            switch (latestChange.Name)
            {
                case EntityName.Customer:
                    var layout = CustomerCollection.First(customer => customer.Id.Equals(latestChange.Id));
                    layout.MoveFromSubmittedToWorking();
                    break;

                case EntityName.LegalEntity:
                    var legalEntityLayout = LegalEntityCollection.First(legalEnity => legalEnity.Id.Equals(latestChange.Id));
                    legalEntityLayout.MoveFromSubmittedToWorking();

                    break;
            }
        }

        internal bool MarkAsReady(ISubmittedEntity workingCopy)
        {
            switch (workingCopy.Name)
            {
                case EntityName.Customer:
                    var layout = CustomerCollection.First(item => item.Id.Equals(workingCopy.Id));
                    return layout.TryMoveFromWorkingCopyToReadyCopy(workingCopy);

                case EntityName.LegalEntity:
                    var legalEntityLayout = LegalEntityCollection.First(item => item.Id.Equals(workingCopy.Id));
                    return legalEntityLayout.TryMoveFromWorkingCopyToReadyCopy(workingCopy);
            }

            return true;
        }

        internal void DiscardWorkingCopy(ISubmittedEntity workingCopy)
        {
            switch (workingCopy.Name)
            {
                case EntityName.Customer:
                    var layout = CustomerCollection.First(item => item.Id.Equals(workingCopy.Id));
                    layout.RemoveFromWorkingCopy(workingCopy);
                    break;

                case EntityName.LegalEntity:
                    var legalEntityLayout = LegalEntityCollection.First(item => item.Id.Equals(workingCopy.Id));
                    legalEntityLayout.RemoveFromWorkingCopy(workingCopy);
                    break;
            }
        }

        internal void MoveLatestToSubmitted(string entityId, int version, string entityName)
        {
            switch (entityName)
            {
                case EntityName.Customer:
                    var customerLayout = CustomerCollection.FirstOrDefault(customer => customer.Id.Equals(entityId, StringComparison.Ordinal));
                    customerLayout?.MovebackToSubmitted(version);

                    break;
                case EntityName.LegalEntity:
                    var legalEntityLayout = LegalEntityCollection.FirstOrDefault(legalEntity => legalEntity.Id.Equals(entityId, StringComparison.Ordinal));
                    legalEntityLayout?.MovebackToSubmitted(version);

                    break;
            }
        }

        internal void CopyReadyToSubmitted(string id, int version, string entityName)
        {
            switch (entityName)
            {
                case EntityName.Customer:
                    var customerLayout = CustomerCollection.FirstOrDefault(customer => customer.Id.Equals(id, StringComparison.Ordinal));
                    var customer = (Customer)customerLayout?.ReadyCopy.Clone();
                    customer.State = EntityState.Submitted; // Bit of a hack
                    customerLayout.SetLatestSubmittedCopy(customer);

                    break;
                case EntityName.LegalEntity:
                    var legalEntityLayout = LegalEntityCollection.FirstOrDefault(legalEntity => legalEntity.Id.Equals(id, StringComparison.Ordinal));
                    var legalEntity = (LegalEntity)legalEntityLayout?.ReadyCopy.Clone();
                    legalEntity.State = EntityState.Submitted; // Bit of a hack
                    legalEntityLayout.SetLatestSubmittedCopy(legalEntity);

                    break;
            }
        }

        internal bool IsBeingEvaluated(string id, string entityName)
        {
            switch (entityName)
            {
                case EntityName.Customer:
                    var customerLayout = CustomerCollection.FirstOrDefault(customer => customer.Id.Equals(id, StringComparison.Ordinal));

                    return customerLayout.WorkingCopy != null && customerLayout.WorkingCopy.Count != 0;

                case EntityName.LegalEntity:
                    var legalEntityLayout = LegalEntityCollection.FirstOrDefault(legalEntity => legalEntity.Id.Equals(id, StringComparison.Ordinal));

                    return legalEntityLayout.WorkingCopy != null && legalEntityLayout.WorkingCopy.Count != 0;
            }

            return false;
        }

        internal bool HasReadyCopy(string entityId, string entityName)
        {
            switch (entityName)
            {
                case EntityName.Customer:
                    var customerLayout = CustomerCollection.FirstOrDefault(customer => customer.Id.Equals(entityId, StringComparison.Ordinal));

                    return customerLayout.ReadyCopy != null;

                case EntityName.LegalEntity:
                    var legalEntityLayout = LegalEntityCollection.FirstOrDefault(legalEntity => legalEntity.Id.Equals(entityId, StringComparison.Ordinal));

                    return legalEntityLayout.ReadyCopy != null;
            }

            return false;
        }

        internal void UpsertDocument<T>(Document<T> document) where T : class, IEntity, new()
        {
            switch (document.Name)
            {
                case EntityName.Customer:
                    var alreadyStored = CustomerDocuments.Any(doc => doc.Id.Equals(document.Id));
                    if (!alreadyStored)
                    {
                        CustomerDocuments.Add((Document<Customer>)Convert.ChangeType(document, typeof(Document<Customer>)));
                    }
                    break;
            }

            throw new NotImplementedException();
        }
    }

    public class EntityLayout<T, U>
        where T : IEntity
        where U : IClientEntity
    {
        public string Id { get; set; }

        /// <summary>
        /// Client is free to update or change this entity version.
        /// As changes are made, the "DraftVersion" increments.
        /// </summary>
        public U ClientCopy { get; set; }

        /// <summary>
        /// This is the entity that the client has submitted. It also also
        /// the latest "Draft" version. Once an entity is copied from 
        /// ClientCopy to LatestSubmittedCopy, then LastSubmittedVersion property
        /// in the ClientCopy is updated. E.g. DraftVersion=3, LastSubmittedVersion=2
        /// which means changes LatestSubmittedCopy contains upto DraftVersion 2.
        /// </summary>
        public T? LastestSubmittedCopy { get; set; }

        /// <summary>
        /// Once Orchestrator is ready to process the LatestSubmittedVersion, then
        /// it is moved from LatestSubmittedCopy into WorkingCopy. Whilst an entity
        /// is in WorkingCopy, its State may change. For example, State may
        /// change to Evaluating -> Applying etc. 
        /// </summary>
        public IList<T?> WorkingCopy { get; set; }

        /// <summary>
        /// Once a workflow has run to end (success or not), then the entity will
        /// move into ReadyCopy. Having an entity in ReadyCopy does not mean
        /// it is ready to be used. Client must consider the State to determine
        /// whether it is ready to be consumed. 
        /// </summary>
        public T? ReadyCopy { get; set; }

        public void MoveFromSubmittedToWorking()
        {
            WorkingCopy ??= new List<T?>();

            WorkingCopy.Add((T)LastestSubmittedCopy);
            LastestSubmittedCopy = default(T);
        }

        public void SetLatestSubmittedCopy(IEntity entity)
        {
            LastestSubmittedCopy = (T)entity;
        }

        public void RemoveFromWorkingCopy(IEntity entity)
        {
            if (WorkingCopy == null || WorkingCopy.Count == 0)
            {
                return;
            }

            WorkingCopy.Remove((T)entity);
        }

        public bool TryMoveFromWorkingCopyToReadyCopy(IEntity entity)
        {
            int itemAtIndex = 0;
            for (var i = 0; i < WorkingCopy.Count; i++)
            {
                if ((IEntity)WorkingCopy[i] == entity)
                {
                    itemAtIndex = i;
                }
            }

            lock (this)
            {

                if (ReadyCopy != null)
                {
                    var changeVersion = ((ISubmittedEntity)entity).SubmittedVersion;
                    var readyCopyVersion = ((ISubmittedEntity)ReadyCopy).SubmittedVersion;

                    if (readyCopyVersion > changeVersion)
                    {
                        // Ready copy is higer than change copy.
                        return false;
                    }
                }

                ReadyCopy = (T)entity;
                WorkingCopy.RemoveAt(itemAtIndex);
            }

            return true;
        }

        internal void MovebackToSubmitted(int version)
        {
            if (LastestSubmittedCopy != null) return; // There is already a submitted copy.

            if (WorkingCopy?.Count == 0)
            {
                if (LastestSubmittedCopy == null)
                {
                    // I need to copy from ReadyCopy to LastestSubmittedCopy.
                    LastestSubmittedCopy = ReadyCopy;
                    LastestSubmittedCopy.State = EntityState.Submitted; // Bit of a hack
                    return;
                }
            }

            // This is ok for now, as we only have one working copy.
            var copyToMove = WorkingCopy.FirstOrDefault();
            if (copyToMove == null)
            {
                return;
            }

            // Submitted entity will have the latest version.
            LastestSubmittedCopy = LastestSubmittedCopy == null ? copyToMove : LastestSubmittedCopy;
            LastestSubmittedCopy.State = EntityState.Submitted; // Bit of a hack
            WorkingCopy.Remove(copyToMove);
        }
    }
}
