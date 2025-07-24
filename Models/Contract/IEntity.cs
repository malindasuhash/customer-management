using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Contract
{
    public interface IEntity // Marker interface
    {
        string Name { get; }
    }

    public class EntityBase : IEntity
    {
        public string Name => nameof(EntityBase);
    }

    public class NullEntity : IEntity
    {
        public static readonly NullEntity Empty = new NullEntity();
        public string Id { get; set; } = Guid.Empty.ToString();
        public string State { get; set; } = string.Empty;
        public string Name => "NullEntity";
        public override string ToString()
        {
            return $"Id: '{Id}', State: '{State}', Name: '{Name}'";
        }
    }
}
