using Models.Contract;
using Models.Infrastructure.Events;
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
        public string Name => EntityName.LegalEntity;
        public string LegalName { get; set; }
    }

    public class LegalEntityClient : LegalEntityBase, IClientEntity, IVersionable
    {
        public int DraftVersion { get; set; }
        public int LastSubmittedVersion { get; set; }
        public int SubmittedVersion { get; set; }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
    public class LegalEntity : LegalEntityBase, ISubmittedEntity, IVersionable
    {
        public int SubmittedVersion { get; set; }
        public IEventInfo GetChangedEvent() => new LegalEntityChanged(Id, SubmittedVersion);
        public override string ToString()
        {
            return string.Format("Id:'{0}', " +
                "CustomerId:'{1}', " +
                "State:'{2}', " +
                "LegalName:'{3}', " +
                "SubmittedVersion:'{4}'",
                Id, CustomerId,  State, LegalName, SubmittedVersion);
        }
    }
}
