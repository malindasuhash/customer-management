using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CustomerBase : IEntity
    {
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public string State { get; set; }
    }

    public class Customer : CustomerBase, IEntity, IVersionable
    {
        public int SubmittedVersion { get; set; }

        public override string ToString()
        {
            return string.Format("Id:'{0}', " +
                "EmailAddress:'{1}', " +
                "State:'{2}', " +
                "SubmittedVersion:'{3}'", 
                Id, EmailAddress, State, SubmittedVersion);
        }
    }

    public class CustomerClient : CustomerBase, IEntity, ICloneable, IClientVersion
    {
        public int DraftVersion { get; set; }

        public int LastSubmittedVersion { get; set; }

        public object Clone() // Clone to get ready for submission
        {
            return new Customer
            {
                Id = Id,
                EmailAddress = EmailAddress,
                State = State
            };
        }

        public override string ToString()
        {
            return string.Format("Id:'{0}', " +
                "EmailAddress:'{1}', " +
                "State:'{2}', " +
                "DraftVersion: '{3}', " +
                "LastSubmittedVersion:'{4}'",
                Id, EmailAddress, State, DraftVersion, LastSubmittedVersion);
        }
    }
}
