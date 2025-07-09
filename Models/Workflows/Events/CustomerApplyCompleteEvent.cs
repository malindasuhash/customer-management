using Models.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Workflows.Events
{
    internal class CustomerApplyCompleteEvent : IEventInfo
    {
        public string EventName => nameof(CustomerApplyCompleteEvent);

        public string CustomerId { get; }

        public int Version { get; }

        public bool Success { get; }

        public CustomerApplyCompleteEvent(string customerId, int version, bool success)
        {
            CustomerId = customerId;
            Version = version;
            Success = success;
        }

        public override string ToString()
        {
            return string.Format("Event='{0}', CustomerId='{1}', Version='{2}', Success:'{3}'", EventName, CustomerId, Version, Success);
        }
    }
}
