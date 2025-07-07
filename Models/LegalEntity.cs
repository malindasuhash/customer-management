using Models.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class LegalEntityBase : IEntity
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }  
        public string State { get; set; }
        public string Name => EnityName.LegalEntity;
        public string LegalName { get; set; }
    }

    public class LegalEntity : LegalEntityBase, IClientEntity, IVersionable
    {
        public int DraftVersion { get; set; }
        public int LastSubmittedVersion { get; set; }

        public int SubmittedVersion { get; set; }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
