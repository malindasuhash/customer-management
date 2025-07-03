using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure.Events
{
    public class CustomerChanged : IEventInfo
    {
        public string EventName => nameof(CustomerChanged);

        public string CustomerId { get; }

        public int Version { get; }

        public CustomerChanged(string customerId, int version)
        {
            CustomerId = customerId;
            Version = version;
        }

        public override string ToString()
        {
            return string.Format("Event='{0}', CustomerId='{1}', Version='{2}'", EventName, CustomerId, Version);
        }
    }
}
