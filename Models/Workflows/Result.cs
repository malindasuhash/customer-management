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

        public static Result EvaluationSuccess(string id, int version) => new(id, version, true, Workflow.Evaluation);

        public static Result ApplySuccess(string id, int version) => new(id, version, true, Workflow.Apply);

        private Result(string id, int version, bool success, Workflow workflow)
        {
            Id = id;
            Version = version;
            Success = success;
            Workflow = workflow;
        }
    }

    internal enum Workflow
    {
        None,
        Evaluation,
        Apply
    }
}
