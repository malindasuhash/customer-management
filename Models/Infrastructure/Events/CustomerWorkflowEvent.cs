using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure.Events
{
    internal class CustomerWorkflowEvent : IWorkflowEvent
    {
        public CustomerWorkflowEvent(string id, int submittedVersion)
        {
            Id = id;
            SubmittedVersion = submittedVersion;
        }

        public string Id { get; }
        public int SubmittedVersion { get; }
    }
}
