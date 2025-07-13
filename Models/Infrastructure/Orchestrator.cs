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
        private readonly EntityChangeHandler _changeHandler = new();

        private static string GenerateLockKey(string entityId, string entityName) => $"EntityLock_{entityId}_{entityName}";

        private void Evaluate(string entityId, string entityName)
        {
            var latestEntityChange = Database.Instance.GetLatestSubmittedEntity(entityId, entityName);
            if (latestEntityChange == null) return;

            _entityManager.Transition(latestEntityChange);

            // TODO: Should I let concurrent changes to the same entity to follow through?
            _outbox.Evaluate(latestEntityChange);
        }

        internal void OnNotify(Result result)
        {
            var workingCopy = Database.Instance.GetWorkingCopy(result.Id, result.Version, result.EntityName);

            if (result.NextAction == NextAction.AwaitingDependency)
            {
                Database.Instance.MoveLatestToSubmitted(result.Id, result.Version, result.EntityName);

                return;
            }

            if (result.NextAction == NextAction.Resubmit)
            {
                // The move the working copy back to submitted
                // Database.Instance.MoveWorkingCopyBackToSubmitted(result.Id, result.Version, result.EntityName);

                // _changeHandler.Manage(workingCopy, true);

                return;
            }

            if (result.Workflow == Workflow.Evaluation && (result.NextAction == NextAction.Apply || result.NextAction == NextAction.Complete))
            {
                // Evaluation succeeded, process next
                switch (result.NextAction)
                {
                    case NextAction.Apply:
                        _entityManager.Transition(workingCopy);
                        _outbox.Apply(workingCopy);
                        break;

                    case NextAction.Complete:
                        _entityManager.Transition(workingCopy, TransitionContext.Completed);
                        _outbox.Ready(workingCopy);
                        break;
                }
            }

            if (result.Workflow == Workflow.Evaluation && !result.Success)
            {
                _entityManager.Transition(workingCopy, TransitionContext.Failed);
                _outbox.WorkingCopyfailed(workingCopy);
            }

            if (result.Workflow == Workflow.Apply && result.Success)
            {
                _entityManager.Transition(workingCopy);
                _outbox.Ready(workingCopy);
            }

            ManageLock(result.Id, result.EntityName, result.Version);
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
            ProcessEntity(result.Id, result.EntityName, result.Version, true);
        }

        private void ManageLock(string entityId, string entityName, int version)
        {
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
                }
            }
        }

        internal void ProcessEntity(string entityId, string entityName, int version, bool isTouched = false)
        {
            var entityLock = GenerateLockKey(entityId, entityName);
            //lock (entityLock)
            //{
            //    // This is to prevent concurrent processing of the same entity
            //    if (_entityIds.TryGetValue(entityLock, out var locks))
            //    {
            //        if (!locks.Contains(version))
            //        {
            //            // Add the version to the list of locks
            //            _entityIds[entityLock].Add(version);
            //            EventAggregator.Log($"<magenta> {entityName} Entity {entityId} with version {version} Queued for processing.");
            //            return;
            //        }
            //    }
            //    else
            //    {
            //        _entityIds[entityLock] = new List<int> { version };
            //    }
            //}

            if (isTouched)
            {
                // The move the working copy back to submitted
                Database.Instance.MoveLatestToSubmitted(entityId, version, entityName);
            }

            // Copy submitted entities to working copy
            _changeHandler.MoveFromSubmittedToWorkingCopy(entityId, entityName, version);

            var workingCopy = Database.Instance.GetWorkingCopy(entityId, version, entityName);
            EventAggregator.Publish(workingCopy.GetChangedEvent());
        }
    }
}
