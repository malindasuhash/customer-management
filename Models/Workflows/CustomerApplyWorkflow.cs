using Models.Infrastructure;
using Models.Infrastructure.Events;
using Models.Workflows.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Workflows
{
    internal class CustomerApplyWorkflow : IWorkflow
    {
        public void Run(IEventInfo eventInfo)
        {
            var customerEvent = (CustomerEvaluationCompleteEvent)eventInfo;

            EventAggregator.Log("START: CustomerApplyWorkflow - Customer Id:'{0}', Version:{1}", customerEvent.CustomerId, customerEvent.Version);

            var workingCopy = Database.Instance.CustomerCollection.First(entry => entry.Id.Equals(customerEvent.CustomerId)).WorkingCopy.First(ver => ver.SubmittedVersion == eventInfo.Version);

            EventAggregator.Log("CustomerApplyWorkflow - Applying change to Identity & Authorisation system for Customer:'{0}'", workingCopy.Id); Thread.Sleep(3000);

            // Notify Orchestrator
            Orchestrator.Instance.OnNotify(Result.ApplySuccess(workingCopy.Id, workingCopy.SubmittedVersion));

            EventAggregator.Log("END: CustomerApplyWorkflow - Customer Id:'{0}', Version: {1}", customerEvent.CustomerId, customerEvent.Version);
        }
    }
}
