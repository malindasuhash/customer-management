using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    internal interface IVersionable
    {
        int SubmittedVersion { get; } 
    }

    internal interface IClientVersion
    {
        int DraftVersion { get; }
        int LastSubmittedVersion { get; }
    }
}
