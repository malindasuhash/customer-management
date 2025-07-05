using Models.Infrastructure;
using Models.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Workflows
{
    internal class CustomerEvaluationWorkflow : IWorkflow
    {
        public void Run(IEventInfo eventInfo)
        {
            var customerEvent = (CustomerChanged)eventInfo;

            EventAggregator.Log("START: CustomerEvaluationWorkflow - Customer Id:'{0}', Version:{1}", customerEvent.CustomerId, customerEvent.Version);

            EventAggregator.Log("CustomerEvaluationWorkflow - Notifying Identity & Authorisation system"); Thread.Sleep(3000);

            // TODO: Update state in entity.

            // TODO: Notify Orchestrator to update entity state.

            EventAggregator.Log("END: CustomerEvaluationWorkflow - Customer Id:'{0}', Version: {1}", customerEvent.CustomerId, customerEvent.Version);
        }
    }
}
