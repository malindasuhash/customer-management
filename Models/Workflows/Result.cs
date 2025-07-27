using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Workflows
{
    internal class Result
    {
        public string Id { get; }
        public int Version { get; }
        public bool Success { get; }
        public Workflow Workflow { get; }
        public NextAction NextAction { get; }
        public string EntityName { get; }

        public static Result EvaluationSuccess(string id, int version, string entityName) => new(id, version, true, Workflow.Evaluation, NextAction.Apply, entityName);

        public static Result EvaluationWaitingDependency(string id, int version, string entityName) => new(id, version, true, Workflow.Evaluation, NextAction.AwaitingDependency, entityName);

        public static Result EvaluationSuccessAndComplete(string id, int version, string entityName) => new(id, version, true, Workflow.Evaluation, NextAction.Complete, entityName);

        public static Result EvaluationFailed(string id, int version, string entityName) => new(id, version, false, Workflow.Evaluation, NextAction.None, entityName);

        public static Result EvaluationContext(string id, int version, string entityName) => new(id, version, false, Workflow.Evaluation, NextAction.None, entityName);

        public static Result ApplySuccess(string id, int version, string entityName) => new(id, version, true, Workflow.Apply, NextAction.None, entityName);

        public static Result ReEvaluate(string id, string entityName) => new(id, 0, false, Workflow.None, NextAction.RequireReEvaluation, entityName);

        internal static Result Evaluate(string customerId, string entityName)
        {
            var result = new Result(customerId, 0, false, Workflow.Evaluation, NextAction.RequireEvaluation, entityName);
            return result;
        }

        private Result(string id, int version, bool success, Workflow workflow, NextAction nextAction, string entityName = null)
        {
            Id = id;
            Version = version;
            Success = success;
            Workflow = workflow;
            NextAction = nextAction;
            EntityName = entityName;
        }
    }

    internal enum Workflow
    {
        None,
        Evaluation,
        Apply
    }

    public enum NextAction
    {
        None,
        Apply,
        Complete,
        RequireInput,
        RequireEvaluation,
        RequireReEvaluation,
        AwaitingDependency,
        Failed
    }
}
