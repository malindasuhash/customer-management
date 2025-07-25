using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Workflows.Events
{
    public interface IWorkflowEvent
    {
        string EventName { get; }
        string EntityId { get; }
        string EntityName { get; }
    }
}
