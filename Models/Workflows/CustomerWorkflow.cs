using Models.Infrastructure;
using Models.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Workflows
{
    internal class CustomerWorkflow : IWorkflow
    {
        public void Run(IEventInfo eventInfo)
        {
            var customerEvent = (CustomerChanged)eventInfo;

            EventAggregator.Log("START: Customer workflow - Customer Id:'{0}', Version:{1}", customerEvent.CustomerId, customerEvent.Version);

            EventAggregator.Log("END: Customer workflow - Customer Id:'{0}', Version: {1}", customerEvent.CustomerId, customerEvent.Version);
        }
    }
}
