using Models.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Infrastructure
{
    public class Document<T> where T : class, IEntity, new()
    {
        public string Id { get; set; }
        public string Name => typeof(T).Name;

        public IEntity Draft { get; set; } = new T();
        public int DraftVersion { get; set; } = 0;

        public IEntity Submitted { get; set; } = new T();
        public int SubmittedVersion { get; set; } = 0;

        public IEntity Approved { get; set; } = new T();
        public int ApprovedVersion { get; set; } = 0;

        public State CurrentState { get; set; } = State.New;
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
