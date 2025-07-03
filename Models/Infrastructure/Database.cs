using System;
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
        public Database()
        {
            CustomerCollection = new List<EntityLayout<Customer>>();
        }

        public IList<EntityLayout<Customer>> CustomerCollection { get; private set; }

        public bool TryAdd(Customer customer)
        {
            throw new NotImplementedException();
        }

        internal Customer GetLatestSubmittedCustomer(string customerId)
        {
            var latestChange = CustomerCollection
                .First(customer => customer.Id.Equals(customerId))
                .WorkingCopy.Pop();

            return latestChange;
        }
    }

    public class EntityLayout<T> where T : IEntity
    {
        public string Id { get; set; }

        public T ClientCopy { get; set; }

        public Stack<T> WorkingCopy { get; set; }

        public T ReadyCopy { get; set; }
    }
}
