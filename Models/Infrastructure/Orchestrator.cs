using Models.Infrastructure.Events;
using Models.Workflows;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public class Orchestrator
    {
        public static Orchestrator Instance = new();

        private Orchestrator(){ }

        private readonly EntityManager _entityManager = new();
        private readonly Outbox _outbox = new();

        public void EntitySubmitted(string entityId, string entityName)
        {
            // TODO: This is where the access to customer may need to be serialised
            var latestCustomerChange = Database.Instance.GetLatestSubmittedCustomer(entityId);
            _entityManager.Transition(latestCustomerChange);
            
            // TODO: Should I let concurrent changes to the same entity to follow through?
            _outbox.Evaluate(latestCustomerChange);
        }

        internal void OnNotify(Result result)
        {
            var workingCopy = Database.Instance.GetWorkingCopy(result.Id, result.Version);

            if (result.Workflow == Workflow.Evaluation && result.NextAction == NextAction.Apply)
            {
                _entityManager.Transition(workingCopy);
                _outbox.Apply(workingCopy);
                return;
            } 

            if (result.Workflow == Workflow.Evaluation && !result.Success)
            {
                _entityManager.Transition(workingCopy, result.Success);
                _outbox.WorkingCopyfailed(workingCopy);
            }

            if (result.Workflow == Workflow.Apply && result.Success)
            {
                _entityManager.Transition(workingCopy);
                _outbox.Ready(workingCopy);
            }
        }

    }
}
