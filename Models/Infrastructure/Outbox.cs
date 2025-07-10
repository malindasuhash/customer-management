using Models.Contract;
using Models.Infrastructure.Events;
using Models.Workflows;
using Models.Workflows.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public class Outbox
    {
        public void NewClientCopy(IClientEntity clientEntity)
        {
            Database.Instance.AddToClientCopy(clientEntity);
        }

        public void UpdateClientCopy(IClientEntity clientEntity)
        {
            Database.Instance.UpdateClientCopy(clientEntity);
        }

        public void JustSubmit(ISubmittedEntity submittedEntity)
        {
            Database.Instance.AddToSubmittedCopy(submittedEntity);
        }

        public void Submit(ISubmittedEntity submittedEntity)
        {
            Database.Instance.AddToSubmittedCopy(submittedEntity);

            EventAggregator.Publish(new EntitySubmitted(submittedEntity.Id, submittedEntity.Name, submittedEntity.SubmittedVersion));
        }

        internal void Evaluate(ISubmittedEntity latestEntityChange)
        {
            Database.Instance.MarkAsWorkingCopy(latestEntityChange);

            // Trigger event
            EventAggregator.Publish(latestEntityChange.GetChangedEvent());
        }

        internal void Apply(ISubmittedEntity workingCopy)
        {
            // Update state

            // Trigger event
            switch (workingCopy.Name)
            {
                case EntityName.Customer:
                    EventAggregator.Publish(new CustomerEvaluationCompleteEvent(workingCopy.Id, workingCopy.SubmittedVersion, true));
                    break;

                case EntityName.LegalEntity:
                    var legalEntity = (LegalEntity)workingCopy;
                    EventAggregator.Publish(new LegalEntityEvaluationCompleteEvent(legalEntity.CustomerId, legalEntity.Id, legalEntity.SubmittedVersion, true));
                    break;
            }
        }

        internal void Ready(ISubmittedEntity workingCopy)
        {
            var readyUpdateResult = Database.Instance.MarkAsReady(workingCopy);

            switch (workingCopy.Name)
            {
                case EntityName.Customer:
                    EventAggregator.Publish(new CustomerSynchonised(workingCopy.Id, workingCopy.SubmittedVersion));
                    break;
                case EntityName.LegalEntity:
                    var legalEntity = (LegalEntity)workingCopy;
                    //EventAggregator.Publish(new LegalEntitySynchonised(legalEntity.CustomerId, legalEntity.Id, legalEntity.SubmittedVersion));
                    break;
            }
        }

        internal void WorkingCopyfailed(ISubmittedEntity workingCopy)
        {
            Database.Instance.MarkAsReady(workingCopy);

            EventAggregator.Publish(new CustomerEvalidationFailed(workingCopy.Id, workingCopy.SubmittedVersion));
        }

        internal void DiscardWorkingCopy(ISubmittedEntity workingCopy)
        {
            Database.Instance.DiscardWorkingCopy(workingCopy);
        }

        internal void MoveFromSubmittedToWorking(ISubmittedEntity submittedEntity)
        {
            Database.Instance.MarkAsWorkingCopy(submittedEntity);
        }
    }
}
