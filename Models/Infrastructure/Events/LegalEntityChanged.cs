using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure.Events
{
    public class LegalEntityChanged : IEventInfo
    {
        public string EventName => nameof(LegalEntityChanged);

        public int Version { get; }

        public string LegalEntityId { get; }

        public LegalEntityChanged(string legalEntityId, int version)
        {
            LegalEntityId = legalEntityId;
            Version = version;
        }

        public override string ToString()
        {
            return string.Format("Event='{0}', LegalEntityId='{1}', Version='{2}'", EventName, LegalEntityId, Version);
        }
    }
}
