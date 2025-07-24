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

        public List<IDocument<Customer>> CustomerDocuments { get; private set; }
        public List<IDocument<LegalEntity>> LegalEntityDocuments { get; private set; } = new();

        static Database()
        {
            Instance.CustomerDocuments = new List<IDocument<Customer>>();
            Instance.LegalEntityDocuments = new List<IDocument<LegalEntity>>();
        }

        public IList<IDocument<IEntity>> GetLatestDraft(string entityId, string entityName)
        {
            var latestDraft = new List<IDocument<IEntity>>();
            IDocument<IEntity>? customer = null;

            if (EntityName.Customer.Equals(entityName, StringComparison.OrdinalIgnoreCase))
            {
                customer = (IDocument<IEntity>?)CustomerDocuments.FirstOrDefault(client => client.Id.Equals(entityId, StringComparison.Ordinal));

                latestDraft.Add(customer);

                var legalEntities = LegalEntityDocuments
                   .Where(client => client.Draft.CustomerId.Equals(customer.Id));
                   
                if (legalEntities != null && legalEntities.Any())
                {
                    foreach (var le in legalEntities)
                    {
                        latestDraft.Add((IDocument<IEntity>)le);
                    }
                }
            }

            return latestDraft;
        }

        internal ISubmittedEntity? GetLatestSubmittedEntity(string entityId, string entityName)
        {
            //ISubmittedEntity returnThis = null;
            //switch (entityName)
            //{
            //    case EntityName.Customer:
            //        returnThis = CustomerCollection.FirstOrDefault(customer => customer.Id.Equals(entityId, StringComparison.Ordinal))?.LastestSubmittedCopy;
            //        break;

            //    case EntityName.LegalEntity:
            //        returnThis = LegalEntityCollection.FirstOrDefault(legalEntity => legalEntity.Id.Equals(entityId, StringComparison.Ordinal))?.LastestSubmittedCopy;
            //        break;
            //}

            //if (returnThis == null)
            //{
            //    var aa = "d";
            //}

            //return returnThis;
            return null;
        }

        internal ISubmittedEntity? GetWorkingCopy(string id, int version, string entityName)
        {
            //switch (entityName)
            //{
            //    case EntityName.Customer:
            //        return CustomerCollection.FirstOrDefault(customer => customer.Id.Equals(id, StringComparison.Ordinal))?.WorkingCopy?.FirstOrDefault();

            //    case EntityName.LegalEntity:
            //        return LegalEntityCollection.FirstOrDefault(legalEntity => legalEntity.Id.Equals(id, StringComparison.Ordinal))?.WorkingCopy?.FirstOrDefault();
            //}

            return null;
        }

        internal void MarkAsWorkingCopy(ISubmittedEntity latestChange)
        {
            //switch (latestChange.Name)
            //{
            //    case EntityName.Customer:
            //        var layout = CustomerCollection.First(customer => customer.Id.Equals(latestChange.Id));
            //        layout.MoveFromSubmittedToWorking();
            //        break;

            //    case EntityName.LegalEntity:
            //        var legalEntityLayout = LegalEntityCollection.First(legalEnity => legalEnity.Id.Equals(latestChange.Id));
            //        legalEntityLayout.MoveFromSubmittedToWorking();

            //        break;
            //}
        }

        internal bool MarkAsReady(ISubmittedEntity workingCopy)
        {
            //switch (workingCopy.Name)
            //{
            //    case EntityName.Customer:
            //        var layout = CustomerCollection.First(item => item.Id.Equals(workingCopy.Id));
            //        return layout.TryMoveFromWorkingCopyToReadyCopy(workingCopy);

            //    case EntityName.LegalEntity:
            //        var legalEntityLayout = LegalEntityCollection.First(item => item.Id.Equals(workingCopy.Id));
            //        return legalEntityLayout.TryMoveFromWorkingCopyToReadyCopy(workingCopy);
            //}

            return true;
        }

        internal void MoveLatestToSubmitted(string entityId, int version, string entityName)
        {
            //switch (entityName)
            //{
            //    case EntityName.Customer:
            //        var customerLayout = CustomerCollection.FirstOrDefault(customer => customer.Id.Equals(entityId, StringComparison.Ordinal));
            //        customerLayout?.MovebackToSubmitted(version);

            //        break;
            //    case EntityName.LegalEntity:
            //        var legalEntityLayout = LegalEntityCollection.FirstOrDefault(legalEntity => legalEntity.Id.Equals(entityId, StringComparison.Ordinal));
            //        legalEntityLayout?.MovebackToSubmitted(version);

            //        break;
            //}
        }

        internal void CopyReadyToSubmitted(string id, int version, string entityName)
        {
        //    switch (entityName)
        //    {
        //        case EntityName.Customer:
        //            var customerLayout = CustomerCollection.FirstOrDefault(customer => customer.Id.Equals(id, StringComparison.Ordinal));
        //            var customer = (Customer)customerLayout?.ReadyCopy.Clone();
        //            customer.State = EntityState.Submitted; // Bit of a hack
        //            customerLayout.SetLatestSubmittedCopy(customer);

        //            break;
        //        case EntityName.LegalEntity:
        //            var legalEntityLayout = LegalEntityCollection.FirstOrDefault(legalEntity => legalEntity.Id.Equals(id, StringComparison.Ordinal));
        //            var legalEntity = (LegalEntity)legalEntityLayout?.ReadyCopy.Clone();
        //            legalEntity.State = EntityState.Submitted; // Bit of a hack
        //            legalEntityLayout.SetLatestSubmittedCopy(legalEntity);

        //            break;
        //    }
        }

        internal bool IsBeingEvaluated(string id, string entityName)
        {
            //switch (entityName)
            //{
            //    case EntityName.Customer:
            //        var customerLayout = CustomerCollection.FirstOrDefault(customer => customer.Id.Equals(id, StringComparison.Ordinal));

            //        return customerLayout.WorkingCopy != null && customerLayout.WorkingCopy.Count != 0;

            //    case EntityName.LegalEntity:
            //        var legalEntityLayout = LegalEntityCollection.FirstOrDefault(legalEntity => legalEntity.Id.Equals(id, StringComparison.Ordinal));

            //        return legalEntityLayout.WorkingCopy != null && legalEntityLayout.WorkingCopy.Count != 0;
            //}

            return false;
        }

        internal bool HasReadyCopy(string entityId, string entityName)
        {
            //switch (entityName)
            //{
            //    case EntityName.Customer:
            //        var customerLayout = CustomerCollection.FirstOrDefault(customer => customer.Id.Equals(entityId, StringComparison.Ordinal));

            //        return customerLayout.ReadyCopy != null;

            //    case EntityName.LegalEntity:
            //        var legalEntityLayout = LegalEntityCollection.FirstOrDefault(legalEntity => legalEntity.Id.Equals(entityId, StringComparison.Ordinal));

            //        return legalEntityLayout.ReadyCopy != null;
            //}

            return false;
        }

        internal void UpsertDocument<T>(IDocument<T> document) where T : class, IEntity, new()
        {
            switch (document.Name)
            {
                case EntityName.Customer:
                    var alreadyStored = CustomerDocuments.Any(doc => doc.Id.Equals(document.Id));
                    if (!alreadyStored)
                    {
                        CustomerDocuments.Add((IDocument<Customer>)Convert.ChangeType(document, typeof(IDocument<Customer>)));
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
    }
}
