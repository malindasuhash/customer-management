using Models.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public interface IDocument<T> where T : IEntity
    {
        public string Id { get; set; }
        public string Name => typeof(T).Name;

        public T Draft { get; set; }
        public int DraftVersion { get; set; }

        public T Submitted { get; set; }
        public int SubmittedVersion { get; set; }

        public T Approved { get; set; }
        public int ApprovedVersion { get; set; }

        public State CurrentState { get; set; }
    }

    public enum State
    {
        New,
        Draft,
        Submitted,
        Approved,
        Rejected
    }
}
