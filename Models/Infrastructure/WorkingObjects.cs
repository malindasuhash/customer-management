using Models.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public class WorkingObjects<T> where T : IEntity
    {
        public T Copy { get; set; }

        Dictionary<Object, object> _objectGraph = new();

        public void AddToGraph<U>(U objectToAdd)
        {
            _objectGraph.Add(typeof(U), objectToAdd);
        }

        public object GetFromGraph<U>()
        {
            if (_objectGraph.TryGetValue(typeof(U), out var value))
            {
                return value;
            }

            return null;
        }
    }
}
