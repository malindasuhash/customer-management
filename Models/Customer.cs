using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Customer : IEntity, ICloneable, IVersionable
    {
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public string State { get; set; }

        public int DraftVersion { get; set; }

        public int SubmittedVersion { get; set; }

        public int LastSubmittedVersion { get; set; }

        public object Clone() // Deep clone
        {
            return new Customer { 
                Id = Id, 
                EmailAddress = EmailAddress, 
                State = State, 
                SubmittedVersion = SubmittedVersion, 
                LastSubmittedVersion = LastSubmittedVersion 
            };
        }
    }
}
