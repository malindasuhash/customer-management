using Models.Contract;
using Models.Infrastructure.Events;
using Models.Workflows;
using Models.Workflows.Events;
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

        // This is a collection of entity IDs that are currently being processed
        private ConcurrentDictionary<string, List<int>> _entityIds = new();

        private Orchestrator() { }

        private readonly EntityManager _entityManager = new();
        private readonly Outbox _outbox = new();
        private readonly EntityChangeHandler _entityChangeHandler = new();

        private static string GenerateLockKey(string entityId, string entityName) => $"EntityLock_{entityId}_{entityName}";

        public void EntitySubmitted(string entityId, string entityName, int version)
        {
            var entityLock = GenerateLockKey(entityId, entityName);
            lock (entityLock)
            {
                // This is to prevent concurrent processing of the same entity
                if (_entityIds.TryGetValue(entityLock, out var locks))
                {
                    if (!locks.Contains(version))
                    {
                        // Add the version to the list of locks
                        _entityIds[entityLock].Add(version);
                        EventAggregator.Log($"<magenta> {entityName} Entity {entityId} with version {version} Queued for processing.");
                        return;
                    }
                }
                else
                {
                    _entityIds[entityLock] = new List<int> { version };
                }

                // Kick off the workflow for this entity

                var latestEntityChange = Database.Instance.GetLatestSubmittedEntity(entityId, entityName, version);
                _entityManager.Transition(latestEntityChange);

                // TODO: Should I let concurrent changes to the same entity to follow through?
                _outbox.Evaluate(latestEntityChange);
            }
        }

        internal void OnNotify(Result result)
        {
            var workingCopy = Database.Instance.GetWorkingCopy(result.Id, result.Version, result.EntityName);

            if (result.Workflow == Workflow.Evaluation && result.NextAction == NextAction.Apply)
            {
                // Evaluation succeeded, remove lock for this entity
                // thereafter see whether there are further actions to take.
                // If there are, then re-evalidate by triggering EntitySubmitted.

                LockAndAction(result.Id, result.EntityName, result.Version);

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

        internal void Touch(Result result, Result executionContext)
        {
            var entityLock = GenerateLockKey(executionContext.Id, executionContext.EntityName);

            // Remove the lock for execution entity
            lock (entityLock)
            {
                if (_entityIds.TryGetValue(entityLock, out var locks))
                {
                    locks.Remove(executionContext.Version);
                    if (locks.Count == 0)
                    {
                        _entityIds.TryRemove(entityLock, out _);
                    }
                }
            }

            // Trigger the evaluation workflow based on the result
            switch (result.EntityName)
            {
                case EntityName.Customer:
                    var latestDraftCustomer = Database.Instance.CustomerCollection.First(c => c.Id.Equals(result.Id)).ClientCopy;
                    _entityChangeHandler.Change(latestDraftCustomer, true);
                    break;

                case EntityName.LegalEntity:
                    var latestDraftLegalEntity = Database.Instance.LegalEntityCollection.First(c => c.Id.Equals(result.Id)).ClientCopy;
                    _entityChangeHandler.Change(latestDraftLegalEntity, true);
                    break;
            }
        }

        private void LockAndAction(string entityId, string entityName, int version)
        {
            var workingCopy = Database.Instance.GetWorkingCopy(entityId, version, entityName);
            if (workingCopy == null)
            {
                EventAggregator.Log($"<red> No working copy found for Entity {entityName} with Id {entityId} and Version {version}.");
                return;
            }

            var entityLock = GenerateLockKey(entityId, entityName);
            lock (entityLock)
            {
                // Remove the lock for this entity
                if (_entityIds.TryGetValue(entityLock, out var locks))
                {
                    locks.Remove(version);
                    if (locks.Count == 0)
                    {
                        _entityIds.TryRemove(entityLock, out _);
                    }
                    else
                    {
                        // Pick the first version to continue processing
                        EventAggregator.Log($"<magenta> New change for {workingCopy.Name} Entity {workingCopy.Id} with version {locks.First()} is ready for processing.");
                        _outbox.DiscardWorkingCopy(workingCopy);

                        // Re-evaluate the working copy
                        EntitySubmitted(workingCopy.Id, workingCopy.Name, locks.First());
                        return; // No need to continue processing this result
                    }
                }
            }
        }
    }
}
