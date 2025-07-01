using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    internal interface IVersionable
    {
        int DraftVersion { get; }
        int SubmittedVersion { get; } 
        int LastSubmittedVersion { get; }
    }
}
