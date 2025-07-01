using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public static class VersionIncrementor
    {
        public static int Increment(this int valueToIncrement)
        {
            return valueToIncrement++;
        }
    }
}
