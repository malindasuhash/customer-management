using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure.Events
{
    public class EntitySubmitted : IEventInfo
    {
        public string EventName => nameof(EntitySubmitted);

        public string EntityId { get; }
        public string EntityName { get; }
        public int Version { get; }

        public EntitySubmitted(string entityId, string entityName, int version)
        {
            EntityId = entityId;
            EntityName = entityName;
            Version = version;
        }
    }
}
