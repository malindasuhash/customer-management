using Models.Contract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public class Database
    {
        public static Database Instance = new();

        private Database()
        {
            CustomerCollection = new List<EntityLayout<Customer, CustomerClient>>();
        }

        public IList<EntityLayout<Customer, CustomerClient>> CustomerCollection { get; private set; }

        public void AddToClientCopy(IClientEntity clientEntity)
        {
            var name = clientEntity.GetType().Name.Replace("Client", string.Empty);

            switch (name)
            {
                case "Customer":
                    CustomerCollection.Add(new EntityLayout<Customer, CustomerClient> { Id = clientEntity.Id, ClientCopy = (CustomerClient)clientEntity });
                    break;
            }

            EventAggregator.Log("Client Entity:'{0}' with Id:'{1}' Added", name, clientEntity.Id);
        }

        public void UpdateClientCopy(IClientEntity clientEntity)
        {
            var name = clientEntity.GetType().Name.Replace("Client", string.Empty);

            switch (name)
            {
                case "Customer":
                    var layout = CustomerCollection.First(customer => customer.Id.Equals(clientEntity.Id));
                    layout.ClientCopy = (CustomerClient)clientEntity;
                    break;
            }

            EventAggregator.Log("Client Entity:'{0}' with Id:'{1}' Updated", name, clientEntity.Id);
        }

        public List<IClientEntity> GetDraftEntitiesFor(string id)
        {
            // TODO: Build up entire object hirarchy
            return CustomerCollection
                .Where(client => client.Id == id && client.ClientCopy.State.Equals(EntityState.Draft))
                .Select(a => (IClientEntity)a.ClientCopy)
                .ToList(); // Run now
        }

        internal void AddToSubmittedCopy(ISubmittedEntity entitytoSubmit)
        {
            var name = entitytoSubmit.GetType().Name;

            switch (name)
            {
                case "Customer":
                    var layout = CustomerCollection.First(customer => customer.Id.Equals(entitytoSubmit.Id));
                    layout.SetLatestSubmittedCopy(entitytoSubmit);
                    break;
            }

            EventAggregator.Log("Submitted Entity:'{0}' with Id:'{1}' Added", name, entitytoSubmit.Id);
        }

        internal Customer GetLatestSubmittedCustomer(string customerId)
        {
            var latestChange = CustomerCollection
                .First(customer => customer.Id.Equals(customerId))
                .LastestSubmittedCopy;

            return latestChange;
        }

        internal Customer GetWorkingCopy(string id, int version)
        {
            var workingCopy = CustomerCollection.First(customer => customer.Id.Equals(id))
                .WorkingCopy.First(ver => ver.SubmittedVersion == version);

            return workingCopy;
        }

        internal void MarkAsWorkingCopy(Customer latestCustomerChange)
        {
            var name = latestCustomerChange.GetType().Name;

            switch (name)
            {
                case "Customer":
                    var layout = CustomerCollection.First(customer => customer.Id.Equals(latestCustomerChange.Id));
                    layout.MoveFromSubmittedCopyToWorkingCopy(latestCustomerChange);
                    break;
            }
        }

        internal bool MarkAsReady(Customer workingCopy)
        {
            var name = workingCopy.GetType().Name;

            switch (name)
            {
                case "Customer":
                    var layout = CustomerCollection.First(item => item.Id.Equals(workingCopy.Id));
                    return layout.TryMoveFromWorkingCopyToReadyCopy(workingCopy);
            }

            return true;
        }

        internal void DiscardWorkingCopy(Customer workingCopy)
        {
            var name = workingCopy.GetType().Name;

            switch (name)
            {
                case "Customer":
                    var layout = CustomerCollection.First(item => item.Id.Equals(workingCopy.Id));
                    layout.RemoveFromWorkingCopy(workingCopy);
                    break;
            }
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

        public void MoveFromSubmittedCopyToWorkingCopy(IEntity entity)
        {
            WorkingCopy ??= new List<T?>();

            WorkingCopy.Add((T)entity);
            LastestSubmittedCopy = default;
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

            lock (this) {

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
