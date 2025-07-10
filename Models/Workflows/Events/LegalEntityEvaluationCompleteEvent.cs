using Models.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Workflows.Events
{
    internal class LegalEntityEvaluationCompleteEvent : IEventInfo
    {
        public string EventName => nameof(LegalEntityEvaluationCompleteEvent);

        public string CustomerId { get; }
        public string LegalEntityId { get; }
        public int Version { get; }

        public bool Success { get; }

        public LegalEntityEvaluationCompleteEvent(string customerId, string legalEntityId, int version, bool success)
        {
            CustomerId = customerId;
            LegalEntityId = legalEntityId;
            Version = version;
            Success = success;
        }

        public override string ToString()
        {
            return string.Format("Event='{0}', CustomerId='{1}', LegalEntityId='{4}' Version='{2}', Success:'{3}'", EventName, CustomerId, Version, Success, LegalEntityId);
        }
    }
}
