using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Contract
{
    public interface IEntity // Marker interface
    {
        string Id { get; set; }
        string State { get; set; }
        string Name { get; }
    }
}
