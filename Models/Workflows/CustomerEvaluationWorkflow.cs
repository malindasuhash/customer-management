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

            EventAggregator.Log("<magenta> START: CustomerEvaluationWorkflow - Customer Id:'{0}', Version:{1}", customerEvent.CustomerId, customerEvent.Version);

            var workingCopy = Database.Instance.CustomerCollection.First(entry => entry.Id.Equals(customerEvent.CustomerId)).WorkingCopy.First(ver => ver.SubmittedVersion == eventInfo.Version);

            if (workingCopy.EmailAddress.Contains("bad", StringComparison.InvariantCultureIgnoreCase))
            {
                EventAggregator.Log("CustomerEvaluationWorkflow - bad email:'{0}' for Customer:'{1}'", workingCopy.EmailAddress, workingCopy.Id); Thread.Sleep(1000);

                // Notify Orchestrator.
                Orchestrator.Instance.OnNotify(Result.EvaluationFailed(workingCopy.Id, workingCopy.SubmittedVersion));
            } else
            {
                // Random delay with email
                var secondsToWait = workingCopy.EmailAddress.Contains("wait") ? int.Parse(workingCopy.EmailAddress.Replace("wait", string.Empty)) : 1;
                EventAggregator.Log("CustomerEvaluationWorkflow - valid email:'{0}' for Customer:'{1}'", workingCopy.EmailAddress, workingCopy.Id); Thread.Sleep(secondsToWait * 1000);

                // Notify Orchestrator.
                Orchestrator.Instance.OnNotify(Result.EvaluationSuccess(customerEvent.CustomerId, customerEvent.Version));
            }

            EventAggregator.Log("END: CustomerEvaluationWorkflow - Customer Id:'{0}', Version: {1}", customerEvent.CustomerId, customerEvent.Version);
        }
    }
}
