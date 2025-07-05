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

        public void EntitySubmitted(ISubmittedEntity entitytoSubmit)
        {
            var latestCustomerChange = Database.Instance.GetLatestSubmittedCustomer(entitytoSubmit.Id);
            _entityManager.Transition(latestCustomerChange);

            // TODO: This is where the access for customer may need to be serialised

            _outbox.ReadyForEvalution(latestCustomerChange);
        }

        internal void OnNotify(Result result)
        {
            var workingCopy = Database.Instance.GetLatestWorkingCopy(result.Id);

            if (result.Workflow == Workflow.Evaluation && result.Success)
            {
                // TODO: Set working copy applying, then start workflow
                _entityManager.Transition(workingCopy);
                _outbox.ReadyToApply(workingCopy);
                
            } else
            {
                // TODO: Set entity as Failed
            }
        }

    }
}
