using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    /// <summary>
    /// Emulates Collection of objections 
    /// </summary>
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
    }

    public class EntityLayout<T, U>
        where T : IEntity
        where U : IClientEntity
    {
        public string Id { get; set; }

        public U ClientCopy { get; set; }

        public T? LastestSubmittedCopy { get; set; }

        public T? WorkingCopy { get; set; }

        public T? ReadyCopy { get; set; }

        public void MoveFromSubmittedCopyToWorkingCopy(IEntity entity)
        {
            WorkingCopy = (T)entity;
            LastestSubmittedCopy = default;
        }

        public void SetLatestSubmittedCopy(IEntity entity)
        {
            LastestSubmittedCopy = (T)entity;
        }
    }
}
