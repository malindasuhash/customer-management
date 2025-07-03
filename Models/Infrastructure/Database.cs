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
            CustomerCollection = new List<Customer>();
        }

        public IList<Customer> CustomerCollection { get; private set; }

        public bool TryAdd(Customer customer)
        {
            throw new NotImplementedException();
        }


    }

    internal class EntityLayout<T> where T : IEntity
    {
        public string EntityName { get; private set; }

        public T ClientCopy { get; set; }

        public T WorkingCopy { get; set; }

        public T Ready { get; set; }
    }
}
